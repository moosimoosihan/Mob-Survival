using UnityEngine;

namespace olimsko
{
    public interface ITableData<TKey>
    {
        TKey GetKey();
        void SetDataFromRow(string[,] data, int row);
    }
}