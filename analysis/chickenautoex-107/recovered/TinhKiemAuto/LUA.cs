namespace TinhKiemAuto
{
	public class LUA
	{
		private Game game;

		public int Index
		{
			set
			{
				Win.PostMessage(game.Handle, Global.HookMessage, value, 56);
			}
		}

		public LUA(Game game)
		{
			this.game = game;
		}

		public void GameProduceLoginMoveToCharacter(int index)
		{
			Index = index;
			Win.PostMessage(game.Handle, Global.HookMessage, 35, 105);
		}

		public void QuestFrameMissionComplete(int index)
		{
			Index = index;
			Win.PostMessage(game.Handle, Global.HookMessage, 36, 105);
		}

		public void SelectRoleEnterGame()
		{
			Win.PostMessage(game.Handle, Global.HookMessage, 37, 105);
		}

		public void TheFireStove_FireButton_OnClick()
		{
			Win.PostMessage(game.Handle, Global.HookMessage, 38, 105);
		}

		public void TheFireStove_StoneButton_OnClick()
		{
			Win.PostMessage(game.Handle, Global.HookMessage, 39, 105);
		}

		public void TheFireStove_MessageBox_OK_Clicked()
		{
			Win.PostMessage(game.Handle, Global.HookMessage, 40, 105);
		}

		public void Play_Ani(int index)
		{
			Index = index;
			Win.PostMessage(game.Handle, Global.HookMessage, 41, 105);
		}

		public void LogOnSelectTail(int index)
		{
			Index = index;
			Win.PostMessage(game.Handle, Global.HookMessage, 42, 105);
		}

		public void DataPoolReConnect()
		{
			Win.PostMessage(game.Handle, Global.HookMessage, 43, 105);
		}

		public void LogOn_ExitToSelectServer()
		{
			Win.PostMessage(game.Handle, Global.HookMessage, 44, 105);
		}

		public void SelectServer(int index)
		{
			Index = index;
			Win.PostMessage(game.Handle, Global.HookMessage, 45, 105);
		}

		public void TogleMissionOutline()
		{
			Win.PostMessage(game.Handle, Global.HookMessage, 46, 105);
		}

		public void AskRet2SelServer()
		{
			Win.PostMessage(game.Handle, Global.HookMessage, 47, 105);
		}

		public void PlayerCreateTeamSelf()
		{
			Win.PostMessage(game.Handle, Global.HookMessage, 48, 105);
		}

		public void OpenWindowMissionTrack()
		{
			Win.PostMessage(game.Handle, Global.HookMessage, 49, 105);
		}

		public void HuoDongRiChengNextClick()
		{
			Win.PostMessage(game.Handle, Global.HookMessage, 50, 105);
		}

		public void LoginOverTime()
		{
			Win.PostMessage(game.Handle, Global.HookMessage, 51, 105);
		}

		public void YuanbaoShop(int list, int shop)
		{
			Win.PostMessage(game.Handle, Global.HookMessage, list, 57);
			Win.PostMessage(game.Handle, Global.HookMessage, shop, 58);
			Win.PostMessage(game.Handle, Global.HookMessage, 52, 105);
		}

		public void ToggleYuanbaoShop()
		{
			Win.PostMessage(game.Handle, Global.HookMessage, 53, 105);
		}

		public void OutGhost()
		{
			string lua = "setmetatable(_G, {__index = Packet_Env }); Relive_Out_Ghost();";
			game.LuaDoOneLineString(lua);
		}

		public void ReturnCount()
		{
			Win.PostMessage(game.Handle, Global.HookMessage, 55, 105);
		}

		public void CountNil()
		{
			Win.PostMessage(game.Handle, Global.HookMessage, 56, 105);
		}

		public void Relive()
		{
			game.LuaDoOneLineString("Player:SendReliveMessage_Relive();");
		}
	}
}
