using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChinaStockQuotes.Entity
{
    public class StockTransaction : TDFData
    {
        public float Price { get; set; } //成交价格
        public long Volume { get; set; } //成交数量
        public string BSFlag { get; set; } //买卖方向(买：'B', 卖：'S', 不明：' ')
    }
}
