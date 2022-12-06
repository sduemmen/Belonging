using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerSkills
{
    [Serializable]
    public struct Skill
    {
        public string name;
    
        public int level;

        public float experience;

        public float experienceNeededForNextLevel;

        public float coefficient;
    }

    public List<Skill> m_skills = new List<Skill>();

    public UnityAction skillLevelUpDelegate;


    public int GetLevel(string skillName)
    {
        return m_skills.Find(skill => skill.name.Equals(skillName)).level;
    }

    public bool AddToSkill(string skillName, float experience)
    {
        for (int i = 0; i < m_skills.Count; i++)
        {
            Skill skill = m_skills[i];
            
            if (skill.name.Equals(skillName))
            {
                skill.experience += experience;

                if (skill.experience >= skill.experienceNeededForNextLevel * Mathf.Pow(skill.coefficient, skill.level + 1))
                {
                    skill.level++;
                    skill.experience = 0;
                    MessageHUD.Instance.AddMessage(new MessageHUD.MsgData(MessageHUD.MsgType.Unlock, MessageHUD.MsgPosition.Center, 5, $"Unlocked {skill.name} level: {skill.level}", false, false));
                    skillLevelUpDelegate?.Invoke();
                }

                m_skills[i] = skill;

                return true;
            }
        }

        return false;
    }
}