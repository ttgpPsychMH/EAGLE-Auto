"""Loopback-only metadata service for manual Windows startup tests."""
import argparse
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
import ssl
import time


class Handler(BaseHTTPRequestHandler):
    def do_GET(self):
        path = self.path.split("?", 1)[0]
        status, content_type, body = 200, "text/plain; charset=utf-8", b"Version = 107\n"
        if path == "/new.ini":
            body = b"Version = 108\n"
        elif path == "/malformed.ini":
            body = b"Version = wrong\n"
        elif path == "/duplicate.ini":
            body = b"Version = 107\nVersion = 108\n"
        elif path == "/missing.ini":
            body = b"Other = 107\n"
        elif path == "/parking.ini":
            content_type, body = "text/html", b"<html>Domain parking</html>"
        elif path == "/oversize.ini":
            body = b"Version = 107\nNote = " + b"x" * 32768
        elif path == "/redirect.ini":
            status = 302
        elif path == "/unavailable.ini":
            status, body = 503, b"Unavailable"
        elif path == "/timeout.ini":
            time.sleep(10)
        elif path != "/current.ini":
            status, body = 404, b"Not found"
        self.send_response(status)
        self.send_header("Content-Type", content_type)
        self.send_header("Content-Length", str(len(body)))
        if status == 302:
            self.send_header("Location", "/current.ini")
        self.end_headers()
        try:
            self.wfile.write(body)
        except (BrokenPipeError, ConnectionResetError):
            pass

    def log_message(self, format, *args):
        # Report only local metadata requests; never dump headers or process environment.
        print(format % args)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--port", type=int, default=8765)
    parser.add_argument("--certificate", help="Optional local PEM certificate for TLS rejection tests")
    parser.add_argument("--private-key", help="Local PEM private key; never commit it")
    args = parser.parse_args()
    service = ThreadingHTTPServer(("127.0.0.1", args.port), Handler)
    scheme = "http"
    if args.certificate or args.private_key:
        if not args.certificate or not args.private_key:
            parser.error("TLS tests need both certificate and private-key")
        context = ssl.SSLContext(ssl.PROTOCOL_TLS_SERVER)
        context.load_cert_chain(args.certificate, args.private_key)
        service.socket = context.wrap_socket(service.socket, server_side=True)
        scheme = "https"
    print("Local fixture:", scheme + "://127.0.0.1:" + str(service.server_port) + "/current.ini", flush=True)
    try:
        service.serve_forever()
    except KeyboardInterrupt:
        pass
    finally:
        service.server_close()


if __name__ == "__main__":
    main()
