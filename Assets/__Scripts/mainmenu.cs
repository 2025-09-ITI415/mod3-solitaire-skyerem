using UnityEngine;
 using UnityEngine.SceneManagement;

public class mainmenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void loadProspector(){
          SceneManager.LoadScene(2);
    }

        public void loadPyramid(){
          SceneManager.LoadScene(0);
    }

     public void loadBJ(){
          SceneManager.LoadScene(1);
    }
     public void loadMM(){
          SceneManager.LoadScene(3);
    }

}
