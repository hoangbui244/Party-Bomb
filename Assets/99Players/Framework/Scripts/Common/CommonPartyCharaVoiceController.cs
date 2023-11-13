using System.Collections.Generic;
using UnityEngine;
public class CommonPartyCharaVoiceController {
    private const float VOICE_VOLUME = 0.8f;
    private const int ACTION_VOICE_NUM = 3;
    private const int DAMAGE_VOICE_NUM = 3;
    private static int[] prevActionVoiceNoArray = new int[6]
    {
        -1,
        -1,
        -1,
        -1,
        -1,
        -1
    };
    private static int[] prevDamageVoiceNoArray = new int[6]
    {
        -1,
        -1,
        -1,
        -1,
        -1,
        -1
    };
    public static void PlayActionVoice(int _playerNo) {
        List<int> list = new List<int>();
        for (int i = 0; i < 3; i++) {
            if (i != prevActionVoiceNoArray[_playerNo]) {
                list.Add(i);
            }
        }
        int num = 0;
        if (list.Count > 0) {
            num = list[Random.Range(0, list.Count)];
        }
        switch (num) {
            case 0:
                SingletonCustom<AudioManager>.Instance.CharaVoicePlay("voice_chara_voice_action_0", _loop: false, 0f, 0.8f);
                break;
            case 1:
                SingletonCustom<AudioManager>.Instance.CharaVoicePlay("voice_chara_voice_action_2", _loop: false, 0f, 0.8f);
                break;
            case 2:
                SingletonCustom<AudioManager>.Instance.CharaVoicePlay("voice_chara_voice_action_3", _loop: false, 0f, 0.8f);
                break;
        }
        prevActionVoiceNoArray[_playerNo] = num;
    }
    public static void PlayDamageVoice(int _playerNo) {
        List<int> list = new List<int>();
        for (int i = 0; i < 3; i++) {
            if (i != prevDamageVoiceNoArray[_playerNo]) {
                list.Add(i);
            }
        }
        int num = 0;
        if (list.Count > 0) {
            num = list[Random.Range(0, list.Count)];
        }
        switch (num) {
            case 0:
                SingletonCustom<AudioManager>.Instance.CharaVoicePlay("voice_chara_voice_damage_0", _loop: false, 0f, 0.8f);
                break;
            case 1:
                SingletonCustom<AudioManager>.Instance.CharaVoicePlay("voice_chara_voice_damage_1", _loop: false, 0f, 0.8f);
                break;
            case 2:
                SingletonCustom<AudioManager>.Instance.CharaVoicePlay("voice_chara_voice_damage_3", _loop: false, 0f, 0.8f);
                break;
        }
        prevDamageVoiceNoArray[_playerNo] = num;
    }
}
