// public class PoolManager : MonoBehaviour
// {
//     Dictionary<GameObject, List<GameObject>> objPoolDic = new Dictionary<GameObject, List<GameObject>>();

//     public GameObject Get(GameObject _obj)
//     {
//         GameObject select = null;
//         List<GameObject> tempList;
//         bool ret = objPoolDic.TryGetValue(_obj, out tempList);
//         if (ret == false)
//         {
//             tempList = new List<GameObject>();
//             objPoolDic.Add(_obj, tempList);
//         }

//         foreach (GameObject item in tempList)
//         {
//             if (!item.activeSelf)
//             {
//                 select = item;
//                 select.SetActive(true);
//                 break;
//             }
//         }

//         if (!select)
//         {
//             select = Instantiate(_obj, transform);
//             tempList.Add(select);
//             select.SetActive(true);
//         }

//         return select;
//     }
// }