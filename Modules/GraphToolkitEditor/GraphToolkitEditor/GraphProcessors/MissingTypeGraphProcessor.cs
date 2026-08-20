// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Processor that reports an error for each node or state whose backing type is missing.
    /// </summary>
    class MissingTypeGraphProcessor : GraphProcessor
    {
        const string k_MissingTypeMessage = "Type is missing. This probably happened because the script that defines it was renamed, moved, or deleted.";

        readonly GraphModel m_GraphModel;

        public MissingTypeGraphProcessor(GraphModel graphModel)
        {
            m_GraphModel = graphModel;
        }

        /// <inheritdoc />
        public override BaseGraphProcessingResult ProcessGraph(GraphChangeDescription changes)
        {
            var res = new ErrorsAndWarningsResult();

            foreach (var placeholder in m_GraphModel.Placeholders)
            {
                if (placeholder is NodePlaceholder or StatePlaceholder)
                    res.AddError(k_MissingTypeMessage, placeholder as Model);
            }

            return res;
        }
    }
}
