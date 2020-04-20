using SuperSocket.ProtoBase;

namespace MMIS
{
    public class PackageInfo : IPackageInfo<byte>
    {
        //Flag
        public byte Flag { get; set; }

        //Head
        public ushort Head { get; set; }

        //任务号
        public int TaskID { get; set; }

        //任务步号
        public ushort TaskStepID { get; set; }

        //车辆编号
        public ushort CarID { get; set; }

        public PackageInfo(ushort head,int taskID, ushort taskStepID, ushort carID)
        {
            Head = head;
            TaskID = taskID;
            TaskStepID = taskStepID;
            CarID = carID;
        }

        public PackageInfo(ushort head,int taskID, ushort taskStepID)
        {
            Head = head;
            TaskID = taskID;
            TaskStepID = taskStepID;
        }
        public PackageInfo()
        {
 
        }

        public byte Key
        {
            get { return Flag; }
        }
    }
}
