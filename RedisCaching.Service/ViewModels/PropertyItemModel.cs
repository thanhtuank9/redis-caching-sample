using ProtoBuf;

namespace RedisCaching.Service.ViewModels
{
    [ProtoContract]
    public class PropertyItemModel
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public string Address { get; set; }
    }
}
