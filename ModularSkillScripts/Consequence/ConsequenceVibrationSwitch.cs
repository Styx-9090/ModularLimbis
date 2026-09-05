using Lethe.Patches;

namespace ModularSkillScripts.Consequence;

public class ConsequenceVibrationSwitch : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		if (!Il2CppSystem.Enum.TryParse(circles[1], out BUFF_UNIQUE_KEYWORD buf_keyword)) buf_keyword = BUFF_UNIQUE_KEYWORD.Enhancement;
		bool isEntangled = circles.Length > 2;
		foreach (BattleUnitModel targetModel in modelList)
		{
			if (isEntangled)
			{
				targetModel.PileUpVibrationToSpecial(modular.modsa_unitModel, BUFF_UNIQUE_KEYWORD.VibrationNesting, buf_keyword, modular.battleTiming, ABILITY_SOURCE_TYPE.SKILL, modular.modsa_selfAction);
				continue;
			}
			targetModel.TakeSwitchVibrationToSpecial(modular.modsa_unitModel, buf_keyword, modular.battleTiming, ABILITY_SOURCE_TYPE.SKILL, modular.modsa_selfAction, out _, out _, out _, out _);
		}
	}
}