/*
 * iChart����ϵͳ(��Դ)
 * �Ϻ�����è��Ϣ�������޹�˾
 */

using System;
using System.Collections.Generic;
using System.Text;
using FaceCat;

namespace chart
{
    /// <summary>
    /// ��Ʊ���ݴ���
    /// </summary>
    public class SecurityDataHelper {
        /// <summary>
        /// ��������Դ
        /// </summary>
        /// <param name="chart">��Ʊ�ؼ�</param>
        /// <returns>����Դ</returns>
        public static FCDataTable createDataSource(FCChart chart) {
            FCDataTable dataSource = new FCDataTable();
            dataSource.addColumn(KeyFields.CLOSE_INDEX);
            dataSource.addColumn(KeyFields.HIGH_INDEX);
            dataSource.addColumn(KeyFields.LOW_INDEX);
            dataSource.addColumn(KeyFields.OPEN_INDEX);
            dataSource.addColumn(KeyFields.VOL_INDEX);
            dataSource.addColumn(KeyFields.AMOUNT_INDEX);
            return dataSource;
        }

        /// <summary>
        /// ����ָ��
        /// </summary>
        /// <param name="chart">��Ʊ�ؼ�</param>
        /// <param name="dataSource">����Դ</param>
        /// <param name="text">�ı�</param>
        /// <param name="parameters">����</param>
        public static FCScript createIndicator(FCChart chart, FCDataTable dataSource, String text, String parameters) {
            FCScript script = new FCScript();
            ArrayList<long> sysColors = new ArrayList<long>();
            long[] m_colors = new long[]
            {
                MyColor.USERCOLOR50, 
                MyColor.USERCOLOR51, 
                MyColor.USERCOLOR48, 
                MyColor.USERCOLOR52, 
                MyColor.USERCOLOR53, 
                FCColor.rgb(150, 123, 220),
                FCColor.rgb(75, 137, 220), 
                MyColor.USERCOLOR22, 
                FCColor.rgb(185, 63, 150)
            };
            sysColors.AddRange(m_colors);
            script.setSystemColors(sysColors);
            script.setDataSource(dataSource);
            script.setName("");
            //indicator.FullName = "";
            if (dataSource != null) {
                script.setSourceField(KeyFields.CLOSE, KeyFields.CLOSE_INDEX);
                script.setSourceField(KeyFields.HIGH, KeyFields.HIGH_INDEX);
                script.setSourceField(KeyFields.LOW, KeyFields.LOW_INDEX);
                script.setSourceField(KeyFields.OPEN, KeyFields.OPEN_INDEX);
                script.setSourceField(KeyFields.VOL, KeyFields.VOL_INDEX);
                script.setSourceField(KeyFields.AMOUNT, KeyFields.AMOUNT_INDEX);
                script.setSourceField(KeyFields.CLOSE.Substring(0, 1), KeyFields.CLOSE_INDEX);
                script.setSourceField(KeyFields.HIGH.Substring(0, 1), KeyFields.HIGH_INDEX);
                script.setSourceField(KeyFields.LOW.Substring(0, 1), KeyFields.LOW_INDEX);
                script.setSourceField(KeyFields.OPEN.Substring(0, 1), KeyFields.OPEN_INDEX);
                script.setSourceField(KeyFields.VOL.Substring(0, 1), KeyFields.VOL_INDEX);
                script.setSourceField(KeyFields.AMOUNT.Substring(0, 1), KeyFields.AMOUNT_INDEX);
            }
            IndicatorData indicatorData = new IndicatorData();
            indicatorData.m_parameters = parameters;
            indicatorData.m_script = text;
            script.setTag(indicatorData);
            String constValue = "";
            if (parameters != null && parameters.Length > 0) {
                String[] strs = parameters.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                int strsSize = strs.Length;
                for (int i = 0; i < strsSize; i++) {
                    String str = strs[i];
                    String[] strs2 = str.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    constValue += "const " + strs2[0] + "=" + strs2[3] + ";";
                }
            }
            if (text != null && text.Length > 0) {
                script.setScript(constValue + text);
            }
            return script;
        }

        /// <summary>
        /// ����ʷ����
        /// </summary>
        /// <param name="chart">��Ʊ�ؼ�</param>
        /// <param name="dataSource">����Դ</param>
        /// <param name="indicators">ָ��</param>
        /// <param name="fields">�ֶ�</param>
        /// <param name="historyDatas">��ʷ����</param>
        public static void bindHistoryDatas(FCChart chart, FCDataTable dataSource, List<FCScript> indicators, int[] fields, List<SecurityData> historyDatas) {
            dataSource.clear();
            int size = historyDatas.Count;
            dataSource.setRowsCapacity(size + 10);
            dataSource.setRowsGrowStep(100);
            int columnsCount = dataSource.getColumnsCount();
            for (int i = 0; i < size; i++) {
                SecurityData securityData = historyDatas[i];
                if (dataSource == chart.getDataSource()) {
                    insertData(chart, dataSource, fields, securityData);
                }
                else {
                    double[] ary = new double[columnsCount];
                    ary[0] = securityData.m_close;
                    ary[1] = securityData.m_high;
                    ary[2] = securityData.m_low;
                    ary[3] = securityData.m_open;
                    ary[4] = securityData.m_volume;
                    for (int j = 5; j < columnsCount; j++) {
                        ary[j] = double.NaN;
                    }
                    dataSource.addRow(securityData.m_date, ary, columnsCount);
                }
            }
            int indicatorsSize = indicators.Count;
            for (int i = 0; i < indicatorsSize; i++) {
                indicators[i].onCalculate(0);
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="chart">֤ȯ�ؼ�</param>
        /// <param name="dataSource">����Դ</param>
        /// <param name="fields">�ֶ�</param>
        /// <param name="securityData">֤ȯ����</param>
        /// <returns>����</returns>
        public static int insertData(FCChart chart, FCDataTable dataSource, int[] fields, SecurityData securityData) {
            double close = securityData.m_close, high = securityData.m_high, low = securityData.m_low, open = securityData.m_open, volume = securityData.m_volume, amount = securityData.m_amount;
            if (volume > 0 || close > 0) {
                if (high == 0) {
                    high = close;
                }
                if (low == 0) {
                    low = close;
                }
                if (open == 0) {
                    open = close;
                }
            }
            else {
                close = double.NaN;
                high = double.NaN;
                low = double.NaN;
                open = double.NaN;
                volume = double.NaN;
                amount = double.NaN;
            }
            double date = securityData.m_date;
            dataSource.set(date, fields[4], volume);
            int index = dataSource.getRowIndex(date);
            dataSource.set2(index, fields[0], close);
            dataSource.set2(index, fields[1], high);
            dataSource.set2(index, fields[2], low);
            dataSource.set2(index, fields[3], open);
            dataSource.set2(index, fields[5], amount);
            return index;
        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="chart">��Ʊ�ؼ�</param>
        /// <param name="dataSource">����Դ</param>
        /// <param name="indicators">ָ��</param>
        /// <param name="fields">�ֶ�</param>
        /// <param name="historyDatas">�������ʷ����</param>
        /// <param name="latestData">ʵʱ����</param>
        /// <returns>����</returns>
        public static int insertLatestData(FCChart chart, FCDataTable dataSource, List<FCScript> indicators, int[] fields, SecurityData latestData) {
            if (latestData.m_close > 0 && latestData.m_volume > 0) {
                int indicatorsSize = indicators.Count;
                int index = insertData(chart, dataSource, fields, latestData);
                for (int i = 0; i < indicatorsSize; i++) {
                    indicators[i].onCalculate(index);
                }
                return index;
            }
            else {
                return -1;
            }
        }
    }
}