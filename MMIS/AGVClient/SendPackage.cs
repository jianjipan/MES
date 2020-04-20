using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMIS
{
    class SendPackage
    {
        //头
        private ushort head = 10003;
        //任务号
        private int taskID;
        //任务步号
        private ushort taskStepID=0;
        //任务类型
        private ushort taskType=0;
        //产品类型
        private ushort productType = 0;
        //优先级
        private ushort priority = 1;
        //车辆类型
        private ushort carType = 0;
        //车辆编号
        private ushort carID = 0;
        //后续任务标识
        private ushort nextTask = 0;
        //参数数量
        private ushort parameter = 2;
        //地图节点号1   取货地图节点号
        private ushort mapID1;
        //地图节点号2   卸货地图节点号
        private ushort mapID2;

        //急停信号
        private ushort essignal;

        public ushort ESSignal
        {
            get { return essignal; }
            set { essignal = value; }
        }

        public ushort Head 
        {
            get { return head; }
            set{head=value;}
        }

        public int TaskID
        {
            get { return taskID; }
            set { taskID = value; }
        }

        public ushort TaskStepID
        {
            get { return taskStepID; }
        }
        public ushort TaskType
        {
            get { return taskType; }
        }
        public ushort ProductType
        {
            get { return productType; }
        }
        public ushort Priority
        {
            get { return priority; }
        }
        public ushort CarType
        {
            get { return carType; }
        }
        public ushort CarID
        {
            get { return carID; }
        }
        public ushort NextTask
        {
            get { return nextTask; }
        }
        public ushort Parameter
        {
            get { return parameter; }
        }
        public ushort MapID1   
        {
            get { return mapID1; }
            set { mapID1 = value; }
        }
        public ushort MapID2
        {
            get { return mapID2; }
            set { mapID2 = value; }
        }
    }
}
