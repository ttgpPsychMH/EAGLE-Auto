namespace MultiClientSimulation;

// Owned, offline design candidate. Never referenced by the application.
// The caller supplies synthetic observations and an external permission decision.
internal sealed class LaunchModel
{
    internal readonly record struct Identity(int Pid, long StartTicks);
    internal sealed record Ticket(long Id, string Request, long Deadline);
    private sealed class Pending(Ticket ticket)
    {
        public Ticket Ticket = ticket;
        public bool TimedOut;
    }
    private readonly object sync = new();
    private readonly int capacity;
    private readonly Dictionary<long, Pending> pending = new();
    private readonly Dictionary<Identity, string?> running = new();
    private readonly HashSet<Identity> ready = new();
    private long next;
    public LaunchModel(int capacity)
    {
        if (capacity < 1 || capacity > 64) throw new ArgumentOutOfRangeException(nameof(capacity));
        this.capacity = capacity;
    }
    public int Occupied { get { lock (sync) return running.Count + pending.Count; } }
    public int Ready { get { lock (sync) return ready.Count; } }
    public int UnconfirmedTimeouts { get { lock (sync) return pending.Values.Count(p => p.TimedOut); } }
    public bool Attach(Identity identity)
    {
        Validate(identity);
        lock (sync) return running.TryAdd(identity, null);
    }
    public Ticket? Reserve(string request, long now, long timeout, bool permitted, bool resourceAvailable)
    {
        if (string.IsNullOrWhiteSpace(request)) throw new ArgumentException("Request required.");
        if (now < 0 || timeout <= 0) throw new ArgumentOutOfRangeException(nameof(timeout));
        long deadline = checked(now + timeout);
        lock (sync)
        {
            if (!permitted || !resourceAvailable || running.Count + pending.Count >= capacity
                || pending.Values.Any(p => p.Ticket.Request == request)
                || running.Values.Contains(request)) return null;
            var ticket = new Ticket(++next, request, deadline);
            pending.Add(ticket.Id, new Pending(ticket));
            return ticket;
        }
    }
    public bool Started(Ticket ticket, Identity identity)
    {
        Validate(identity);
        lock (sync)
        {
            if (!pending.TryGetValue(ticket.Id, out var entry) || entry.Ticket != ticket) return false;
            if (running.TryGetValue(identity, out var owner) && owner != null) return false;
            // A discovery can precede start acknowledgement. Merge its identity, do not double-count it.
            pending.Remove(ticket.Id);
            running[identity] = ticket.Request;
            return true;
        }
    }
    public bool ConfirmNoProcess(Ticket ticket)
    {
        lock (sync)
            return pending.TryGetValue(ticket.Id, out var entry) && entry.Ticket == ticket
                && pending.Remove(ticket.Id);
    }
    public void Poll(long now)
    {
        if (now < 0) throw new ArgumentOutOfRangeException(nameof(now));
        lock (sync)
            foreach (var entry in pending.Values)
                if (now >= entry.Ticket.Deadline) entry.TimedOut = true;
        // Timeout is not proof of process absence. Keep its slot until explicit reconciliation.
    }
    public bool WindowReady(Identity identity)
    {
        lock (sync) return running.ContainsKey(identity) && ready.Add(identity);
    }
    public bool Exited(Identity identity)
    {
        lock (sync)
        {
            ready.Remove(identity);
            return running.Remove(identity);
        }
    }
    private static void Validate(Identity identity)
    {
        if (identity.Pid <= 0 || identity.StartTicks <= 0) throw new ArgumentException("Invalid synthetic identity.");
    }
}
