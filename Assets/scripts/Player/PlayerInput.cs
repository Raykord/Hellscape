using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private void Update()
    {
        // Using Input.GetAxis() to read user input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Additional user input handling logic
    }

    //Проверака на любой ввод
    public bool isInput()
    {
        if (Input.anyKey)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetHorizontalInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public float GetVerticalInput()
    {
        return Input.GetAxisRaw("Vertical");
    }

    public bool GetLeftMouseButton()
    {
        return Input.GetMouseButton(0);
    }

    public bool GetSpaceButton()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
}
