using Menu.Remix.MixedUI;
using UnityEngine;

namespace StunMaster.Config
{
    public class OptionsMenu : OptionInterface
    {
        public readonly Configurable<bool> LPC;
        private UIelement[] options;
        public OptionsMenu() => LPC = config.Bind("LPC", true);


        public override void Initialize()
        {
            OpTab optionsTab = new(this, "Options");
            Tabs = [optionsTab];

            options = [
                new OpCheckBox(LPC, new(0f, 475f)),
                new OpLabel(new(0f, 500f), new(600f, 30f), "Disable Stun Powers", FLabelAlignment.Left, true),
            ];

            optionsTab.AddItems(options);
        }
    }
}
