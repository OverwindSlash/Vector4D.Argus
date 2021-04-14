using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace Argus.StereoCalibration.yaml
{
    public class FlowStyleDoubleSequences : ChainedEventEmitter
    {
        public FlowStyleDoubleSequences(IEventEmitter nextEmitter)
            : base(nextEmitter) { }

        public override void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter)
        {
            if (typeof(double[]).IsAssignableFrom(eventInfo.Source.Type))
            {
                eventInfo = new SequenceStartEventInfo(eventInfo.Source)
                {
                    Style = SequenceStyle.Flow
                };
            }

            nextEmitter.Emit(eventInfo, emitter);
        }
    }
}
