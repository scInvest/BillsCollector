namespace WebClient.Components.UIComponents
{
    public class SheetFocusManger
    {
        List<IFocusableObject> _focusableObjects = new List<IFocusableObject>();
        public IReadOnlyCollection<IFocusableObject> FocusableObjects => _focusableObjects;

        public void Register(IFocusableObject focusableObject)
        {
            _focusableObjects.Add(focusableObject);
            focusableObject.Focus += FocusableObject_Focus;
        }

        private void FocusableObject_Focus(object sender, EventArgs e)
        {
            foreach (var obj in _focusableObjects)
            {
                if (obj != sender)
                {
                    obj.RemoveFocus();
                }
            }
        }
    }

    public interface IFocusableObject
    {
        public event EventHandler Focus;

        public void RemoveFocus();
    }
}
