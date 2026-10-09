using System.Collections.Generic;
using TinhKiemAuto;
using TinhKiemAuto.Models;

public class FindPath
{
	private static List<SceneInfo> scenesList = new List<SceneInfo>();

	private static List<Scene> scenes = new List<Scene>();

	private static Scene GetSceneById(int sceneId)
	{
		foreach (Scene scene in scenes)
		{
			if (scene.sceneId == sceneId)
			{
				return scene;
			}
		}
		return null;
	}

	private static SceneInfo FindSceneInsideList(Scene scene)
	{
		foreach (SceneInfo scenes in scenesList)
		{
			if (scenes.scene.sceneId == scene.sceneId)
			{
				return scenes;
			}
		}
		return null;
	}

	private static void ReadData()
	{
		string[] array = LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\16.dat").Split('\n');
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Trim().Split('\t');
			int sceneId = int.Parse(array2[0]);
			string sceneName = array2[1];
			Scene item = new Scene(sceneId, sceneName);
			scenes.Add(item);
		}
		string[] array3 = LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\17.dat").Split('\n');
		for (int j = 1; j < array3.Length; j++)
		{
			string[] array4 = array3[j].Trim().Split('\t');
			int sceneId2 = int.Parse(array4[0]);
			int sceneId3 = int.Parse(array4[2]);
			string npcName = array4[4];
			int teleportationCost = int.Parse(array4[5]);
			int num = int.Parse(array4[6]);
			int num2 = int.Parse(array4[7]);
			Scene sceneById = GetSceneById(sceneId2);
			Scene sceneById2 = GetSceneById(sceneId3);
			SceneInfo sceneInfo = FindSceneInsideList(sceneById);
			if (sceneInfo == null)
			{
				sceneInfo = new SceneInfo(sceneById);
				scenesList.Add(sceneInfo);
			}
			sceneInfo.AddNearScene(sceneById2, num, num2);
			sceneInfo.AddTeleportNPC(new TeleportNPC(npcName, teleportationCost, num, num2));
		}
	}

	public static List<SceneInfo> GetPathsList(int fromSceneId, int toSceneId)
	{
		if (scenesList.Count <= 0)
		{
			ReadData();
		}
		Queue<SceneInfo> queue = new Queue<SceneInfo>();
		SceneInfo[] array = new SceneInfo[1000];
		bool[] array2 = new bool[1000];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = true;
			array[i] = null;
		}
		SceneInfo sceneInfo = FindSceneInsideList(GetSceneById(fromSceneId));
		SceneInfo sceneInfo2 = FindSceneInsideList(GetSceneById(toSceneId));
		queue.Enqueue(sceneInfo);
		array[fromSceneId] = null;
		array2[sceneInfo.scene.sceneId] = false;
		while (queue.Count > 0)
		{
			SceneInfo sceneInfo3 = queue.Dequeue();
			foreach (NearScene nearScene in sceneInfo3.nearScenes)
			{
				if (!array2[nearScene.scene.sceneId])
				{
					continue;
				}
				array2[nearScene.scene.sceneId] = false;
				SceneInfo sceneInfo4 = FindSceneInsideList(nearScene.scene);
				if (sceneInfo4 == null)
				{
					continue;
				}
				queue.Enqueue(sceneInfo4);
				array[sceneInfo4.scene.sceneId] = sceneInfo3;
				if (sceneInfo4 == sceneInfo2)
				{
					List<SceneInfo> list = new List<SceneInfo>();
					SceneInfo sceneInfo5 = sceneInfo4;
					list.Add(sceneInfo5);
					while (array[sceneInfo5.scene.sceneId] != sceneInfo)
					{
						sceneInfo5 = array[sceneInfo5.scene.sceneId];
						list.Add(sceneInfo5);
					}
					list.Add(sceneInfo);
					return list;
				}
			}
		}
		return null;
	}

	public static int GetPortalX(SceneInfo fromScene, SceneInfo toScene)
	{
		foreach (NearScene nearScene in fromScene.nearScenes)
		{
			if (nearScene.scene == toScene.scene)
			{
				return nearScene.nearScenePortalX;
			}
		}
		return -1;
	}

	public static int GetPortalY(SceneInfo fromScene, SceneInfo toScene)
	{
		foreach (NearScene nearScene in fromScene.nearScenes)
		{
			if (nearScene.scene == toScene.scene)
			{
				return nearScene.nearScenePortalY;
			}
		}
		return -1;
	}

	public static TeleportNPC GetTeleportNPC(SceneInfo fromScene, SceneInfo toScene)
	{
		foreach (NearScene nearScene in fromScene.nearScenes)
		{
			if (nearScene.scene != toScene.scene)
			{
				continue;
			}
			foreach (TeleportNPC teleportNPC in fromScene.teleportNPCs)
			{
				if (nearScene.nearScenePortalX == teleportNPC.posX && nearScene.nearScenePortalY == teleportNPC.posY)
				{
					return teleportNPC;
				}
			}
		}
		return null;
	}

	public static PathInfo GetNextPath(int fromSceneId, int toSceneId)
	{
		List<SceneInfo> pathsList = GetPathsList(fromSceneId, toSceneId);
		pathsList.Reverse();
		TeleportNPC teleportNPC = GetTeleportNPC(pathsList[0], pathsList[1]);
		if (teleportNPC == null)
		{
			return new PathInfo(isNPC: false, GetPortalX(pathsList[0], pathsList[1]), GetPortalY(pathsList[0], pathsList[1]), pathsList[1].scene.sceneId, "NONE");
		}
		return new PathInfo(isNPC: true, GetPortalX(pathsList[0], pathsList[1]), GetPortalY(pathsList[0], pathsList[1]), pathsList[1].scene.sceneId, teleportNPC.npcName);
	}

	public static string GetScreenName(int screenID)
	{
		string result = "";
		foreach (Scene scene in scenes)
		{
			if (scene.sceneId == screenID)
			{
				result = scene.sceneName;
				break;
			}
		}
		return result;
	}
}
