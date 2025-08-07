using ROI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseForceMoveEffect : DisplayableEffect,IEffectSystem, IPauseHandle
{
    private Vector3 _direction;
    private StatusParamKeyWord move_type;
    private Vector3 origin;
    private VectorApplicator vectorApplicator; 
    public void ApplyEffect(ChampionData champion, StatusData arg)
    {
        if (vectorApplicator == null)
        {
            vectorApplicator = GetComponent<VectorApplicator>();
        }
        StatusParam current_level = arg.GetCurrentParam();

        if (current_level == null)
        {
            return;
        }

        foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_level.param_list)
        {
            switch (entry.Key)
            {
                case StatusParamKeyWord.KnockUp:
                    ApplyIcon(champion, 1, ChampionEffects.KnockUp, arg.remain_duration, arg.setting.duration);
                    /*float up = entry.Value;
                    StartCoroutine(MoveChampionWithForce(Vector3.up,arg.remain_duration,up,arg,champion.gameObject));
                    move_type = entry.Key;
                    origin = champion.gameObject.transform.position;*/
                    vectorApplicator.vector_force = entry.Value* 200;
                    vectorApplicator.applied_vector = Vector3.up ;
                    vectorApplicator.ApplyVector(arg.target.rigid);
                    break;
                case StatusParamKeyWord.KnockBack:
                    ApplyIcon(champion, 1, ChampionEffects.KnockBack, arg.remain_duration, arg.setting.duration);
                    champion.AddEffect(ChampionEffects.KnockBack);
                    Debug.Log($"Knock back champion with distance {entry.Value}");
                    // float back = entry.Value;
                    move_type = entry.Key;
                    //Vector3 _back = champion.gameObject.transform.forward * -1;
                    Vector3 _back = champion.transform.position - arg.position;
                    _back.y = 0;
                    _back.Normalize();
                    if (Vector3.Distance(champion.transform.position, arg.position) < 0.01f)
                    {
                        _back = champion.transform.forward*-1;
                    }
                    // StartCoroutine(MoveChampionWithForce(_back, arg.remain_duration, back, arg, champion.gameObject));
                    vectorApplicator.vector_force = Mathf.Abs(entry.Value) * 1000;
                    vectorApplicator.applied_vector = _back * entry.Value;
                    vectorApplicator.ApplyVector(arg.target.rigid);
                    break;
                case StatusParamKeyWord.Explosion:
                    break;
            }
        }
        champion.controller.Pause(this);

    }

    public void RemoveEffect(ChampionData champion, StatusData arg)
    {
        Logs.Info("BaseForceMoveEffect => RemoveEffect() => champion: " + champion + " arg: " + arg + " move_type: " + move_type);
        
        StatusParam current_level = arg.GetCurrentParam();
        bool effectHasKnockup = false;
        foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_level.param_list) {
            Logs.Info("BaseForceMoveEffect => RemoveEffect() => removing effect: " + entry.Key);
            var effectType = (ChampionEffects)entry.Key;
            champion.RemoveEffect(effectType);
            if (entry.Key == StatusParamKeyWord.KnockUp) {
                effectHasKnockup = true;
            }
        }
        
        IsPaused = false;
        champion.controller.ResetHexPosition();
        Destroy(gameObject);

        if (effectHasKnockup)
        {
            // Applies a stun effect if the unit landed after a knockup
            // TODO: move the entire physics system to a separate global script as units could be affected by multiple physics effects at the same time and these could stack
            // TODO: Make knockdown apply only if the unit was flying and now landed with a specific force
            GeneralEffectSystem.Instance.ApplyEffect(champion, new StatusData("KnockDown", champion, new Vector3()));
        } 
    }
    
    public void ReApplyEffect(ChampionData champion, StatusData arg)
    {
        
    }

    public bool IsPaused
    {
        get; set;
    }
}
