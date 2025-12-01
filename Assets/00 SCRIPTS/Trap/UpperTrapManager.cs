using UnityEngine;

public class UpperTrapManager : MonoBehaviour
{
    public GameObject trap_1A_Object; 
    public GameObject trap_1B_Object; 

    void Start()
    {
        RandomlyActivateTrap();
    }

    void RandomlyActivateTrap()
    {
        int randomChoice = Random.Range(1, 3); 

        if (randomChoice == 1)
        {
            trap_1A_Object.SetActive(true);
            trap_1B_Object.SetActive(false);
            Debug.Log("Đã chọn Bẫy 1");
        }
        else 
        {
            trap_1A_Object.SetActive(false);
            trap_1B_Object.SetActive(true);
            Debug.Log("Đã chọn Bẫy 2");
        }
    }
}