using System;
using System.Collections.Generic;
using System.Text;
using FaceCat;

namespace chart {
    public class PriceDataServiceClient : FCClientService{
        public PriceDataServiceClient() {
            setServiceID(SERVICEID);
        }

        public const int SERVICEID = 10004;

        /// <summary>
        /// 注册代码
        /// </summary>
        public const int FUNCTION_SUBCODES = 0;

        /// <summary>
        /// 反注册代码
        /// </summary>
        public const int FUNCTION_UNSUBCODES = 1;

        /// <summary>
        /// 最新数据
        /// </summary>
        public const int FUNCTION_NEWDATA = 2;

        private int m_socketID;

        /// <summary>
        /// 获取套接字
        /// </summary>
        /// <returns></returns>
        public int getSocketID() {
            return m_socketID;
        }

        /// <summary>
        /// 设置套接字
        /// </summary>
        /// <param name="value"></param>
        public void setSocketID(int value) {
            m_socketID = value;
        }

        public void subCodes(int requestID, String codes) {
            FCBinary bw = new FCBinary();
            bw.writeString(codes);
            byte[] bytes = bw.getBytes();
            int ret = send(new FCMessage(getServiceID(), FUNCTION_SUBCODES, requestID, m_socketID, 0, 0, bytes.Length, bytes));
            bw.close();
        }

        public void unSubCodes(int requestID) {
            FCBinary bw = new FCBinary();
            bw.writeString("1");
            byte[] bytes = bw.getBytes();
            int ret = send(new FCMessage(getServiceID(), FUNCTION_UNSUBCODES, requestID, m_socketID, 0, 0, bytes.Length, bytes));
            bw.close();
        }

        public static List<PriceData> getPriceDatas(byte[] body, int bodyLength) {
            FCBinary br = new FCBinary();
            br.write(body, bodyLength);
            List<PriceData> priceDatas = new List<PriceData>();
            int datasSize = br.readInt();
            for (int i = 0; i < datasSize; i++) {
                PriceData data = new PriceData();
                data.m_code = br.readString();
                data.m_open = br.readDouble();
                data.m_lastClose = br.readDouble();
                data.m_close = br.readDouble();
                data.m_high = br.readDouble();
                data.m_low = br.readDouble();
                data.m_volume = br.readDouble();
                data.m_amount = br.readDouble();
                data.m_date = br.readDouble();
                priceDatas.Add(data);
            }
            br.close();
            //Console.WriteLine(JsonConvert.SerializeObject(priceDatas));
            return priceDatas;
        }

        /// <summary>
        /// 接收消息方法
        /// </summary>
        /// <param name="message"></param>
        public override void onReceive(FCMessage message) {
            base.onReceive(message);
            if (message.m_functionID == FUNCTION_NEWDATA) {
               
            }
            sendToListener(message);
        }
    }
}
