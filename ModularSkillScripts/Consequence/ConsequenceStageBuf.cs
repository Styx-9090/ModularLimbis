using System;
using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequenceBloodfeast : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int bloodDinner = modular.GetNumFromParamString(circles[1]);
		if (string.Equals(circles[0], "add", StringComparison.OrdinalIgnoreCase)) BloodDinnerBuff.BuffInstance.AddStack(bloodDinner, modular.battleTiming, false);
		else if (string.Equals(circles[0], "sub", StringComparison.OrdinalIgnoreCase)) BloodDinnerBuff.BuffInstance.SubStack(bloodDinner, modular.battleTiming);
		else if (circles.Length > 1 && string.Equals(circles[0], "use", StringComparison.OrdinalIgnoreCase))
		{
			BattleUnitModel targetModel = modular.GetTargetModel(circles[2]);
			BloodDinnerBuff.BuffInstance.UseBuffStack(targetModel, bloodDinner, modular.battleTiming, null);
		}
	}
}

public class ConsequenceStageBuf : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (circles.Length <= 2) {
			MainClass.LogModular("Bozo, Not enough arguments for stagebuf(), fucking idiot", true);
			return;
		}
		
		BattleUnitBuffManager bufManager = Singleton<BattleUnitBuffManager>.Instance;
		if (bufManager == null) {
			MainClass.LogModular("stagebuf() null BattleUnitBuffManager", true);
			return;
		}
		StageBuffManager stageBufManager = bufManager._stageBuffManager;
		
		string keyword_s = circles[0];
		if (!Il2CppSystem.Enum.TryParse(keyword_s, true, out BUFF_UNIQUE_KEYWORD keyword)) {
			MainClass.LogModular("stagebuf() invalid keyword idiot", true);
			return;
		}
		string mode_s = circles[1];
		
		switch (mode_s)
		{
			case "init": {
				stageBufManager.AddStageBuff(keyword);
				if (!stageBufManager.CheckStageBuff(keyword)) {
					StageBuffModel buf_new = new(keyword, 999, 0);
					stageBufManager._stageBuffDictionary[keyword] = buf_new;
				}
				BattleUnitModel unit = modular.GetTargetModel(circles[2]);
				if (unit != null) {
					int instID = unit._instanceID;
					stageBufManager.AddCandidateToKeyword(keyword, instID);
					if (!stageBufManager._buffCandidates.ContainsKey(keyword)) {
						List<int> candidate_list = new();
						candidate_list.Add(instID);
						stageBufManager._buffCandidates[keyword] = candidate_list;
					}
					else if (!stageBufManager._buffCandidates[keyword].Contains(instID)) {
						stageBufManager._buffCandidates[keyword].Add(instID);
					}
				}
			} break;
			case "add": {
				StageBuffModel buf = GetStageBufModel(stageBufManager, keyword);
				if (buf == null) return;
				bool is_turn = circles[2] == "turn";
				int stack = modular.GetNumFromParamString(circles[3]);

				if (!is_turn) {
					if (stack > 0) buf.AddStack(stack, modular.battleTiming, false);
					else if (stack < 0) buf.SubStack(-stack, modular.battleTiming);
				}else {
					if (stack > 0) buf.AddTurn(stack, modular.battleTiming);
					else if (stack < 0) buf.SubTurn(-stack, modular.battleTiming);
				}
			} break;
			case "spend": {
				StageBuffModel buf = GetStageBufModel(stageBufManager, keyword);
				if (buf == null) return;
				
				bool is_turn = circles[2] == "turn";
				int stack = modular.GetNumFromParamString(circles[3]);
				if (stack < 1) return;
				BattleUnitModel unit = modular.GetTargetModel(circles[4]);
				if (unit == null) return;
				
				if (!is_turn) buf.UseBuffStack(unit, keyword, stack, modular.battleTiming);
				else buf.UseBuffTurn(unit, keyword, stack, modular.battleTiming);
			} break;
		}
	}

	public static StageBuffModel GetStageBufModel(StageBuffManager stageBufManager, BUFF_UNIQUE_KEYWORD keyword)
	{
		if (stageBufManager._stageBuffDictionary == null) return null;
		if (stageBufManager._stageBuffDictionary.TryGetValue(keyword, out StageBuffModel buf)) return buf;
		return null;
	}
}

