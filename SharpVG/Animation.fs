﻿namespace SharpVG

open System

type CalculationMode =
    | Discrete = 1
    | Linear = 2
    | Paced = 3
    | Spline = 4

type AttributeType =
    | CSS = 1
    | XML = 2

type Change =
    {
        AttributeName: string
        AttributeValue: string
        AttributeType: AttributeType
    }

type Motion =
    {
        Path: Path
        CalculationMode: CalculationMode option
    }

// TODO: Combine with Transform.fs if possible
type AnimateTransform =
    | Translate of X: Length * Y: Length option
    | Scale of X: Length * Y: Length option
    | Rotate of Angle: float *  X: Length option * Y: Length option
    | SkewX of Angle: float
    | SkewY of Angle: float

type AnimationType =
    | Set of Change
    | Animate of Change
    | Transform of AnimateTransform
    | Motion of Motion

type Animation =
    {
        AnimationType: AnimationType
        Timing: Timing
    }

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Animation =
    let createSet timing attributeType attributeName attributeValue =
        {
            AnimationType = Set {AttributeName = attributeName; AttributeValue = attributeValue; AttributeType = attributeType };
            Timing = timing
        }

    let createAnimation timing attributeType attributeName attributeValue =
        {
            AnimationType = Animate {AttributeName = attributeName; AttributeValue = attributeValue; AttributeType = attributeType };
            Timing = timing
        }

    let createMotion timing path calculationMode =
        {
            AnimationType = Motion {Path = path; CalculationMode = calculationMode};
            Timing = timing
        }

    let toTag animation =
        let name, attribute =
            match animation.AnimationType with 
                | Set c -> "set", "attributeName=" + Tag.quote(c.AttributeName) + " attributeType=" + Tag.quote (Enum.GetName(typeof<AttributeType>, c.AttributeType)) + " to=" + Tag.quote(c.AttributeValue)
                | Animate c -> "animate", "attributeName=" + Tag.quote(c.AttributeName) + " attributeType=" + Tag.quote (Enum.GetName(typeof<AttributeType>, c.AttributeType)) + " to=" + Tag.quote(c.AttributeValue)
                | Transform _ -> "animateTransform", ""
                | Motion m -> "animateMotion", (m.Path |> Path.toAttributeString) + match m.CalculationMode with | Some(c) -> " " + Enum.GetName(typeof<CalculationMode>, c).ToLower() | None -> ""
        Tag.create name 
        |> Tag.addAttribute attribute
        |> Tag.addAttribute (animation.Timing |> Timing.toString)

    let toString animation = animation |> toTag |> Tag.toString