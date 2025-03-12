using System;
using System.Collections.Generic;
using UnityEngine;

public class GroupAI : MonoBehaviour
{
    [Header("Faction Settings")]
    [Tooltip("Tag for this faction (e.g., 'Rock', 'Paper', etc.)")]
    public string factionTag;
    
    [Header("Faction Relations")]
    [Tooltip("Tags of factions that can defeat this faction (enemy).")]
    public List<string> enemyTags;
    [Tooltip("Tags of factions that this faction can defeat (target).")]
    public List<string> targetTags;
    
    [Header("Speed Settings")]
    [SerializeField] private float calmSpeed = 2f;
    [SerializeField] private float averageSpeed = 6f;
    [SerializeField] private float aggressiveSpeed = 2f;
    public float aggressiveness;

    [Header("Friendly Unit Graph Parameters")] 
    [SerializeField] private int lowFriendlies = 2;
    [SerializeField] private int highFriendlies = 7;
    
    [Header("Target Unit Graph Parameters")] 
    [SerializeField] private int lowTargets = 3;
    [SerializeField] private int highTargets = 8;
    
    [Header("Enemy Unit Graph Parameters")] 
    [SerializeField] private int lowEnemies = 1;
    [SerializeField] private int highEnemies = 6;
    private int CountUnits(string targetTag)
    {
        GameObject[] units = GameObject.FindGameObjectsWithTag(targetTag);
        return units.Length;
    }
    
    private int CountUnits(List<string> tags)
    {
        int total = 0;
        foreach (var t in tags)
        {
            total += CountUnits(t);
        }
        return total;
    }

    private float GetLowMembership(int unitCount, int lowerBound, int upperBound)
    {
        if (unitCount <= lowerBound)
        {
            return 1f;
        }

        if (unitCount >= upperBound)
        {
            return 0f;
        }
        
        return Mathf.Round(((upperBound - unitCount) / (float)(upperBound - lowerBound)) * 10f) / 10f;
    }
    
    private float GetHighMembership(int unitCount, int lowerBound, int upperBound)
    {
        if (unitCount <= lowerBound)
        {
            return 0f;
        }

        if (unitCount >= upperBound)
        {
            return 1f;
        }
        
        return Mathf.Round(((unitCount - lowerBound) / (float)(upperBound - lowerBound)) * 10f) / 10f;
    }
    
    private void AddRuleContribution(ref float sumNum, ref float sumDen, float ruleStrength, float ruleSpeed)
    {
        sumNum += ruleStrength * ruleSpeed;
        sumDen += ruleStrength;
    }
    
    private void Awake()
    {
        aggressiveness = averageSpeed;
    }

    private void Update()
    {
        int friendlyUnitCount = CountUnits(factionTag);
        int targetUnitCount = CountUnits(targetTags);
        int enemyUnitCount = CountUnits(enemyTags);
        
        float friendlyLow = GetLowMembership(friendlyUnitCount, lowFriendlies, highFriendlies);
        float friendlyHigh = GetHighMembership(friendlyUnitCount, lowFriendlies, highFriendlies);
        
        float targetLow = GetLowMembership(targetUnitCount, lowTargets, highTargets);
        float targetHigh = GetHighMembership(targetUnitCount, lowTargets, highTargets);
        
        float enemyLow = GetLowMembership(enemyUnitCount, lowEnemies, highEnemies);
        float enemyHigh = GetHighMembership(enemyUnitCount, lowEnemies, highEnemies);
        
        // 1) If (High#EnemyUnits AND High#FriendlyUnits) then average speed
        float rule1 = MathF.Min(enemyHigh, friendlyHigh);
        
        // 2) If (High#EnemyUnits AND Low#FriendlyUnits) then aggressive
        float rule2 = MathF.Min(enemyHigh, friendlyLow);
        
        // 3) If (Low#EnemyUnits OR High#TargetUnits) then average speed
        float rule3 = MathF.Max(enemyLow, targetHigh);
        
        // 4) If (Low#EnemyUnits OR Low#TargetUnits) then move calmly
        float rule4 = MathF.Max(enemyLow, targetLow);
        
        // 5) If (High#TargetUnits OR High#FriendlyUnits) AND NOT High#EnemyUnits then aggressive
        float rule5 = MathF.Min(MathF.Max(targetHigh, friendlyHigh),(1 - enemyHigh));
        
        // 6) If (Low#TargetUnits OR High#FriendlyUnits) AND NOT High#EnemyUnits then average speed
        float rule6 = MathF.Min(MathF.Max(targetLow, friendlyHigh),(1 - enemyHigh));
        
        // 7) If (Low#TargetUnits OR Low#FriendlyUnits) OR Low#EnemyUnits then move calmly
        float rule7 = MathF.Max(MathF.Max(targetLow, friendlyLow), enemyLow);
        
        
        // Defuzzification
        float numerator = 0f;
        float denominator = 0f;
        
        AddRuleContribution(ref numerator, ref denominator, rule1, averageSpeed);
        AddRuleContribution(ref numerator, ref denominator, rule2, aggressiveSpeed);   
        AddRuleContribution(ref numerator, ref denominator, rule3, averageSpeed);
        AddRuleContribution(ref numerator, ref denominator, rule4, calmSpeed);
        AddRuleContribution(ref numerator, ref denominator, rule5, aggressiveSpeed);
        AddRuleContribution(ref numerator, ref denominator, rule6, averageSpeed);
        AddRuleContribution(ref numerator, ref denominator, rule7, calmSpeed);

        float speed = (denominator > 0) ? numerator / denominator : averageSpeed;
        speed = MathF.Min(speed, aggressiveSpeed);
        aggressiveness = speed;
    }
}
