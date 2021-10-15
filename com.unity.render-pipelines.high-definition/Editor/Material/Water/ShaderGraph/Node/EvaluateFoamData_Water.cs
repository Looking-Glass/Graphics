using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEditor.Rendering.HighDefinition
{
    [SRPFilter(typeof(HDRenderPipeline))]
    [Title("Utility", "High Definition Render Pipeline", "Water", "EvaluateFoamData_Water (Preview)")]
    class EvaluateFoamData_Water : AbstractMaterialNode, IGeneratesBodyCode
    {
        public EvaluateFoamData_Water()
        {
            name = "Evaluate Foam Data Water (Preview)";
            UpdateNodeAfterDeserialization();
        }

        public override string documentationURL => Documentation.GetPageLink("EvaluateFoamData_Water");

        const int kSurfaceGradientInputSlotId = 0;
        const string kSurfaceGradientInputSlotName = "SurfaceGradient";

        const int kLowFrequencySurfaceGradientInputSlotId = 1;
        const string kLowFrequencySurfaceGradientInputSlotName = "LowFrequencySurfaceGradient";

        const int kSimulationFoamInputSlotId = 2;
        const string kSimulationFoamInputSlotName = "SimulationFoam";

        const int kLowFrequencyHeightInputSlotId = 3;
        const string kLowFrequencyHeightInputSlotName = "LowFrequencyHeight";

        const int kFoamFromHeightInputSlotId = 4;
        const string kFoamFromHeightInputSlotName = "FoamFromHeight";

        const int kNormalWSInputSlotId = 5;
        const string kNormalWSInputSlotName = "NormalWS";

        const int kPositionWSInputSlotId = 6;
        const string kPositionWSInputSlotName = "PositionWS";

        const int kSurfaceGradientOutputSlotId = 7;
        const string kSurfaceGradientOutputSlotName = "SurfaceGradient";

        const int kFoamOutputSlotId = 8;
        const string kFoamOutputSlotName = "Foam";

        public override bool hasPreview { get { return false; } }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            // Input
            AddSlot(new Vector3MaterialSlot(kSurfaceGradientInputSlotId, kSurfaceGradientInputSlotName, kSurfaceGradientInputSlotName, SlotType.Input, Vector3.zero, ShaderStageCapability.Fragment));
            AddSlot(new Vector3MaterialSlot(kLowFrequencySurfaceGradientInputSlotId, kLowFrequencySurfaceGradientInputSlotName, kLowFrequencySurfaceGradientInputSlotName, SlotType.Input, Vector3.zero, ShaderStageCapability.Fragment));
            AddSlot(new Vector2MaterialSlot(kSimulationFoamInputSlotId, kSimulationFoamInputSlotName, kSimulationFoamInputSlotName, SlotType.Input, Vector2.zero, ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(kLowFrequencyHeightInputSlotId, kLowFrequencyHeightInputSlotName, kLowFrequencyHeightInputSlotName, SlotType.Input, 0, ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(kFoamFromHeightInputSlotId, kFoamFromHeightInputSlotName, kFoamFromHeightInputSlotName, SlotType.Input, 0, ShaderStageCapability.Fragment));
            AddSlot(new Vector3MaterialSlot(kNormalWSInputSlotId, kNormalWSInputSlotName, kNormalWSInputSlotName, SlotType.Input, Vector3.zero, ShaderStageCapability.Fragment));
            AddSlot(new Vector3MaterialSlot(kPositionWSInputSlotId, kPositionWSInputSlotName, kPositionWSInputSlotName, SlotType.Input, Vector3.zero, ShaderStageCapability.Fragment));
            
            // Output
            AddSlot(new Vector3MaterialSlot(kSurfaceGradientOutputSlotId, kSurfaceGradientOutputSlotName, kSurfaceGradientOutputSlotName, SlotType.Output, Vector3.zero));
            AddSlot(new Vector3MaterialSlot(kFoamOutputSlotId, kFoamOutputSlotName, kFoamOutputSlotName, SlotType.Output, Vector3.zero));

            RemoveSlotsNameNotMatching(new[]
            {
                // Input
                kSurfaceGradientInputSlotId,
                kLowFrequencySurfaceGradientInputSlotId,
                kSimulationFoamInputSlotId,
                kLowFrequencyHeightInputSlotId,
                kFoamFromHeightInputSlotId,
                kNormalWSInputSlotId,
                kPositionWSInputSlotId,

                // Output
                kSurfaceGradientOutputSlotId,
                kFoamOutputSlotId,
            });
        }

        public void GenerateNodeCode(ShaderStringBuilder sb, GenerationMode generationMode)
        {
            if (generationMode == GenerationMode.ForReals)
            {
                string surfaceGradient = GetSlotValue(kSurfaceGradientInputSlotId, generationMode);
                string lowFrequencySG = GetSlotValue(kLowFrequencySurfaceGradientInputSlotId, generationMode);
                string simulationFoam = GetSlotValue(kSimulationFoamInputSlotId, generationMode);
                string foamFromHeight = GetSlotValue(kFoamFromHeightInputSlotId, generationMode);
                string lowFrequencyHeight = GetSlotValue(kLowFrequencyHeightInputSlotId, generationMode);
                string normalWS = GetSlotValue(kNormalWSInputSlotId, generationMode);
                string positionWS = GetSlotValue(kPositionWSInputSlotId, generationMode);

                sb.AppendLine("FoamData foamData;");
                sb.AppendLine("ZERO_INITIALIZE(FoamData, foamData);");
                
                sb.AppendLine("EvaluateFoamData({0}, {1}, {2}, {3}, {4}, {5}, {6}, foamData);",
                    surfaceGradient,
                    lowFrequencySG,
                    simulationFoam,
                    lowFrequencyHeight,
                    foamFromHeight,
                    normalWS,
                    positionWS
                );

                sb.AppendLine("$precision3 {0} = foamData.foamValue;",
                    GetVariableNameForSlot(kFoamOutputSlotId)
                );

                sb.AppendLine("$precision3 {0} = foamData.surfaceGradient;",
                    GetVariableNameForSlot(kSurfaceGradientOutputSlotId)
                );
            }
            else
            {
                sb.AppendLine("$precision3 {0} = 0.0;",
                    GetVariableNameForSlot(kFoamOutputSlotId)
                );

                sb.AppendLine("$precision3 {0} = 0.0;",
                    GetVariableNameForSlot(kSurfaceGradientOutputSlotId)
                );
            }
        }
    }
}
