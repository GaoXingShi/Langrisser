using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainSpace.SkillCommandSpace
{
    public interface SkillCommand
    {
        void SetBackSkillCommand(SkillCommand _skillCommand);
        SkillCommand BackSkillCommand();
        void Execute();
        void Cancel();
        void Affirm();
    }

    public class AttackSkillCommand : SkillCommand
    {
        private SkillCommand backSkillCommand;
        public void SetBackSkillCommand(SkillCommand _skillCommand)
        {
            backSkillCommand = _skillCommand;
        }

        public SkillCommand BackSkillCommand()
        {
            return backSkillCommand;
        }

        public void Execute()
        {

        }

        public void Cancel()
        {
            
        }

        public void Affirm()
        {
        }
    }
}
