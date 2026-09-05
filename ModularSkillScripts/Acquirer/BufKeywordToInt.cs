using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Lethe.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerBufKeywordToInt : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (!Il2CppSystem.Enum.TryParse(circledSection, out BUFF_UNIQUE_KEYWORD buf_keyword)) buf_keyword = BUFF_UNIQUE_KEYWORD.Enhancement;
		return (int)buf_keyword;
	}
}
