public class SpearBattle_Define
{
	public enum SkillType
	{
		Empty,
		Rock,
		Scissors,
		Paper,
		Special_Rock,
		Special_Scissors,
		Special_Paper
	}
	public enum PhaseResultType
	{
		Win,
		Lose,
		Draw,
		SpecialWin
	}
	public const int PHASE_NUM = 5;
	public const int SPECIAL_NUM = 1;
	public const int MAX_SAME_SKILL_NUM = 2;
	public const int HP = 5;
	public const int SPECIAL_BIG_DAMAGE = 2;
	public const int BIG_DAMAGE = 1;
	public const int NORMAL_DAMAGE = 1;
	public const float SELECT_TIME = 5f;
	public const float FADE_TIME = 0.5f;
	public const float FADE_BLACK_TIME = 0f;
	public static readonly string[] SkillSpriteNameArray = new string[7]
	{
		"result_hatena",
		"result_goo",
		"result_choki",
		"result_par",
		"result_goo",
		"result_choki",
		"result_par"
	};
	public static readonly string[] PhaseResultSpriteNameArray = new string[4]
	{
		"result_circle",
		"result_x",
		"result_triangle",
		"result_circle"
	};
	public static int CalcDamage(SkillType _attackSkill, SkillType _defenseSkill)
	{
		switch (_attackSkill)
		{
		case SkillType.Rock:
			switch (_defenseSkill)
			{
			case SkillType.Scissors:
			case SkillType.Special_Scissors:
				return 1;
			case SkillType.Rock:
				return 1;
			default:
				return 0;
			}
		case SkillType.Scissors:
			switch (_defenseSkill)
			{
			case SkillType.Paper:
			case SkillType.Special_Paper:
				return 1;
			case SkillType.Scissors:
				return 1;
			default:
				return 0;
			}
		case SkillType.Paper:
			switch (_defenseSkill)
			{
			case SkillType.Rock:
			case SkillType.Special_Rock:
				return 1;
			case SkillType.Paper:
				return 1;
			default:
				return 0;
			}
		case SkillType.Special_Rock:
			switch (_defenseSkill)
			{
			case SkillType.Rock:
			case SkillType.Scissors:
			case SkillType.Special_Scissors:
				return 2;
			case SkillType.Special_Rock:
				return 1;
			default:
				return 0;
			}
		case SkillType.Special_Scissors:
			switch (_defenseSkill)
			{
			case SkillType.Scissors:
			case SkillType.Paper:
			case SkillType.Special_Paper:
				return 2;
			case SkillType.Special_Scissors:
				return 1;
			default:
				return 0;
			}
		case SkillType.Special_Paper:
			switch (_defenseSkill)
			{
			case SkillType.Rock:
			case SkillType.Paper:
			case SkillType.Special_Rock:
				return 2;
			case SkillType.Special_Paper:
				return 1;
			default:
				return 0;
			}
		default:
			return 0;
		}
	}
	public static PhaseResultType CalcPhaseResultType(SkillType _attackSkill, SkillType _defenseSkill)
	{
		switch (_attackSkill)
		{
		case SkillType.Rock:
			switch (_defenseSkill)
			{
			case SkillType.Scissors:
			case SkillType.Special_Scissors:
				return PhaseResultType.Win;
			case SkillType.Rock:
				return PhaseResultType.Draw;
			default:
				return PhaseResultType.Lose;
			}
		case SkillType.Scissors:
			switch (_defenseSkill)
			{
			case SkillType.Paper:
			case SkillType.Special_Paper:
				return PhaseResultType.Win;
			case SkillType.Scissors:
				return PhaseResultType.Draw;
			default:
				return PhaseResultType.Lose;
			}
		case SkillType.Paper:
			switch (_defenseSkill)
			{
			case SkillType.Rock:
			case SkillType.Special_Rock:
				return PhaseResultType.Win;
			case SkillType.Paper:
				return PhaseResultType.Draw;
			default:
				return PhaseResultType.Lose;
			}
		case SkillType.Special_Rock:
			switch (_defenseSkill)
			{
			case SkillType.Rock:
			case SkillType.Scissors:
			case SkillType.Special_Scissors:
				return PhaseResultType.SpecialWin;
			case SkillType.Special_Rock:
				return PhaseResultType.Draw;
			default:
				return PhaseResultType.Lose;
			}
		case SkillType.Special_Scissors:
			switch (_defenseSkill)
			{
			case SkillType.Scissors:
			case SkillType.Paper:
			case SkillType.Special_Paper:
				return PhaseResultType.SpecialWin;
			case SkillType.Special_Scissors:
				return PhaseResultType.Draw;
			default:
				return PhaseResultType.Lose;
			}
		case SkillType.Special_Paper:
			switch (_defenseSkill)
			{
			case SkillType.Rock:
			case SkillType.Paper:
			case SkillType.Special_Rock:
				return PhaseResultType.SpecialWin;
			case SkillType.Special_Paper:
				return PhaseResultType.Draw;
			default:
				return PhaseResultType.Lose;
			}
		default:
			return PhaseResultType.Lose;
		}
	}
	public static int CalcPhaseResultNo(SkillType _attackSkill, SkillType _defenseSkill)
	{
		return (int)CalcPhaseResultType(_attackSkill, _defenseSkill);
	}
}
