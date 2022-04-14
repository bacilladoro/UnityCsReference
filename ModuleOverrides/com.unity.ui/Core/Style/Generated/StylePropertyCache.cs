// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

/******************************************************************************/
//
//                             DO NOT MODIFY
//          This file has been generated by the UIElementsGenerator tool
//              See StylePropertyCacheCsGenerator class for details
//
/******************************************************************************/
using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.StyleSheets
{
    internal static partial class StylePropertyCache
    {
        internal static readonly Dictionary<string, string> s_PropertySyntaxCache = new Dictionary<string, string>()
        {
            {"align-content", "flex-start | flex-end | center | stretch | auto"},
            {"align-items", "flex-start | flex-end | center | stretch | auto"},
            {"align-self", "flex-start | flex-end | center | stretch | auto"},
            {"all", "initial"},
            {"background-color", "<color>"},
            {"background-image", "<resource> | <url> | none"},
            {"border-bottom-color", "<color>"},
            {"border-bottom-left-radius", "<length-percentage>"},
            {"border-bottom-right-radius", "<length-percentage>"},
            {"border-bottom-width", "<length>"},
            {"border-color", "<color>{1,4}"},
            {"border-left-color", "<color>"},
            {"border-left-width", "<length>"},
            {"border-radius", "[ <length-percentage> ]{1,4}"},
            {"border-right-color", "<color>"},
            {"border-right-width", "<length>"},
            {"border-top-color", "<color>"},
            {"border-top-left-radius", "<length-percentage>"},
            {"border-top-right-radius", "<length-percentage>"},
            {"border-top-width", "<length>"},
            {"border-width", "<length>{1,4}"},
            {"bottom", "<length-percentage> | auto"},
            {"color", "<color>"},
            {"cursor", "[ [ <resource> | <url> ] [ <integer> <integer> ]? ] | [ arrow | text | resize-vertical | resize-horizontal | link | slide-arrow | resize-up-right | resize-up-left | move-arrow | rotate-arrow | scale-arrow | arrow-plus | arrow-minus | pan | orbit | zoom | fps | split-resize-up-down | split-resize-left-right ]"},
            {"display", "flex | none"},
            {"flex", "none | [ <'flex-grow'> <'flex-shrink'>? || <'flex-basis'> ]"},
            {"flex-basis", "<'width'>"},
            {"flex-direction", "column | row | column-reverse | row-reverse"},
            {"flex-grow", "<number>"},
            {"flex-shrink", "<number>"},
            {"flex-wrap", "nowrap | wrap | wrap-reverse"},
            {"font-size", "<length-percentage>"},
            {"height", "<length-percentage> | auto"},
            {"justify-content", "flex-start | flex-end | center | space-between | space-around"},
            {"left", "<length-percentage> | auto"},
            {"letter-spacing", "<length>"},
            {"margin", "[ <length-percentage> | auto ]{1,4}"},
            {"margin-bottom", "<length-percentage> | auto"},
            {"margin-left", "<length-percentage> | auto"},
            {"margin-right", "<length-percentage> | auto"},
            {"margin-top", "<length-percentage> | auto"},
            {"max-height", "<length-percentage> | none"},
            {"max-width", "<length-percentage> | none"},
            {"min-height", "<length-percentage> | auto"},
            {"min-width", "<length-percentage> | auto"},
            {"opacity", "<number>"},
            {"overflow", "visible | hidden | scroll"},
            {"padding", "[ <length-percentage> ]{1,4}"},
            {"padding-bottom", "<length-percentage>"},
            {"padding-left", "<length-percentage>"},
            {"padding-right", "<length-percentage>"},
            {"padding-top", "<length-percentage>"},
            {"position", "relative | absolute"},
            {"right", "<length-percentage> | auto"},
            {"rotate", "none | <angle>"},
            {"scale", "none | <number>{1,3}"},
            {"text-overflow", "clip | ellipsis"},
            {"text-shadow", "<length>{2,3} && <color>?"},
            {"top", "<length-percentage> | auto"},
            {"transform-origin", "[ <length> | <percentage> | left | center | right | top | bottom ] | [ [ <length> | <percentage>  | left | center | right ] && [ <length> | <percentage>  | top | center | bottom ] ] <length>?"},
            {"transition", "<single-transition>#"},
            {"transition-delay", "<time>#"},
            {"transition-duration", "<time>#"},
            {"transition-property", "none | <single-transition-property>#"},
            {"transition-timing-function", "<easing-function>#"},
            {"translate", "none | [<length> | <percentage>] [ [<length> | <percentage>] <length>? ]?"},
            {"-unity-background-image-tint-color", "<color>"},
            {"-unity-background-scale-mode", "stretch-to-fill | scale-and-crop | scale-to-fit"},
            {"-unity-font", "<resource> | <url>"},
            {"-unity-font-definition", "<resource> | <url>"},
            {"-unity-font-style", "normal | italic | bold | bold-and-italic"},
            {"-unity-overflow-clip-box", "padding-box | content-box"},
            {"-unity-paragraph-spacing", "<length>"},
            {"-unity-slice-bottom", "<integer>"},
            {"-unity-slice-left", "<integer>"},
            {"-unity-slice-right", "<integer>"},
            {"-unity-slice-scale", "<length>"},
            {"-unity-slice-top", "<integer>"},
            {"-unity-text-align", "upper-left | middle-left | lower-left | upper-center | middle-center | lower-center | upper-right | middle-right | lower-right"},
            {"-unity-text-outline", "<length> || <color>"},
            {"-unity-text-outline-color", "<color>"},
            {"-unity-text-outline-width", "<length>"},
            {"-unity-text-overflow-position", "start | middle | end"},
            {"visibility", "visible | hidden"},
            {"white-space", "normal | nowrap"},
            {"width", "<length-percentage> | auto"},
            {"word-spacing", "<length>"}
        };

        internal static readonly Dictionary<string, string> s_NonTerminalValues = new Dictionary<string, string>()
        {
            {"easing-function", "linear | <timing-function>"},
            {"length-percentage", "<length> | <percentage>"},
            {"single-transition", "[ none | <single-transition-property> ] || <time> || <easing-function> || <time>"},
            {"single-transition-property", "all | <custom-ident>"},
            {"timing-function", "ease | ease-in | ease-out | ease-in-out | ease-in-sine | ease-out-sine | ease-in-out-sine | ease-in-cubic | ease-out-cubic | ease-in-out-cubic | ease-in-circ | ease-out-circ | ease-in-out-circ | ease-in-elastic | ease-out-elastic | ease-in-out-elastic | ease-in-back | ease-out-back | ease-in-out-back | ease-in-bounce | ease-out-bounce | ease-in-out-bounce"}
        };
    }
}
