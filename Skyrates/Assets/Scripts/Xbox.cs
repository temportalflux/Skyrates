using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Input
{

    public enum InputType
    {
        BUTTON, AXIS
    }

    public class Xbox
    {

        public enum Enum
        {
            // Buttons
            A, B, X, Y,
            VIEW, MENU,
            BUMPER_L, BUMPER_R,
            STICK_L, STICK_R,
            // Axes
            TRIGGER_L, TRIGGER_R,
            STICK_L_HORIZONTAL, STICK_L_VERTICAL,
            STICK_R_HORIZONTAL, STICK_R_VERTICAL,
            DPAD_HORIZONTAL, DPAD_VERTICAL,
        }

        public static readonly Dictionary<Enum, Xbox> enumToXbox = new Dictionary<Enum, Xbox>()
        {
            {Enum.A, new Xbox(Enum.A, InputType.BUTTON, "xbox_a")},
            {Enum.B, new Xbox(Enum.B, InputType.BUTTON, "xbox_b")},
            {Enum.X, new Xbox(Enum.X, InputType.BUTTON, "xbox_x")},
            {Enum.Y, new Xbox(Enum.Y, InputType.BUTTON, "xbox_y")},
            {Enum.BUMPER_L, new Xbox(Enum.BUMPER_L, InputType.BUTTON, "xbox_bumper_l")},
            {Enum.BUMPER_R, new Xbox(Enum.BUMPER_R, InputType.BUTTON, "xbox_bumper_r")},
            {Enum.VIEW, new Xbox(Enum.VIEW, InputType.BUTTON, "xbox_button_view")},
            {Enum.MENU, new Xbox(Enum.MENU, InputType.BUTTON, "xbox_button_menu")},
            {Enum.STICK_L, new Xbox(Enum.STICK_L, InputType.BUTTON, "xbox_stick_l_button")},
            {Enum.STICK_R, new Xbox(Enum.STICK_R, InputType.BUTTON, "xbox_stick_r_button")},
            {Enum.STICK_L_HORIZONTAL, new Xbox(Enum.STICK_L_HORIZONTAL, InputType.AXIS, "xbox_stick_l_horizontal")},
            {Enum.STICK_L_VERTICAL, new Xbox(Enum.STICK_L_VERTICAL, InputType.AXIS, "xbox_stick_l_vertical")},
            {Enum.STICK_R_HORIZONTAL, new Xbox(Enum.STICK_R_HORIZONTAL, InputType.AXIS, "xbox_stick_r_horizontal")},
            {Enum.STICK_R_VERTICAL, new Xbox(Enum.STICK_R_VERTICAL, InputType.AXIS, "xbox_stick_r_vertical")},
            {Enum.DPAD_HORIZONTAL, new Xbox(Enum.DPAD_HORIZONTAL, InputType.AXIS, "xbox_dpad_horizontal")},
            {Enum.DPAD_VERTICAL, new Xbox(Enum.DPAD_VERTICAL, InputType.AXIS, "xbox_dpad_vertical")},
            {Enum.TRIGGER_L, new Xbox(Enum.TRIGGER_L, InputType.AXIS, "xbox_trigger_l")},
            {Enum.TRIGGER_R, new Xbox(Enum.TRIGGER_R, InputType.AXIS, "xbox_trigger_r")},
        };

        public static Xbox find(Enum key)
        {
            return enumToXbox[key];
        }

        private readonly Enum key;
        private readonly InputType inputType;
        private readonly string descriptor;

        public Xbox(Enum key, InputType inputType, string descriptor)
        {
            this.key = key;
            this.inputType = inputType;
            this.descriptor = descriptor;
        }

        public Enum getKey()
        {
            return this.key;
        }

        public InputType getInputType()
        {
            return this.inputType;
        }

        public string getInputDescriptor()
        {
            return this.descriptor;
        }

    }

    

    
}
