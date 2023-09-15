/* =======================================================================================================
 * AK Studio
 * 
 * Version 1.0 by Alexandr Kuznecov
 * 04.12.2022
 * =======================================================================================================
 */

using UnityEngine;

[CreateAssetMenu(fileName = "New LocalizationData", menuName = "Localization Data", order = 51)]
public class LangData : ScriptableObject
{
	public string menu_title = "";
	public string menu_start = "";
	public string menu_restart = "";
	public string menu_resume = "";
	public string menu_continue = "";
    public string menu_back = "";
    public string menu_to_main = "";
	public string menu_new_game_confirm = "";
	public string menu_new_game = "";

	public string menu_music_toggle = "";
	public string menu_effect_toggle = "";

	public string text_start_level = "";
	public string text_level = "";
	public string text_target = "";
	public string text_start_steps = "";
	public string text_steps = "";
	public string text_score = "";

    public string text_bonus_level = "";
    public string text_bonus_button = "";

    public string text_win = "";
	public string text_lose = "";
	public string text_no_solution = "";

	public string text_congratulation_1 = "";
	public string text_congratulation_2 = "";
	public string text_congratulation_3 = "";
	public string text_congratulation_4 = "";
	public string text_congratulation_5 = "";

	public static string GetValue(LangData d, string key)
    {
		return (string)typeof(LangData).GetField(key).GetValue(d);
	}
}
