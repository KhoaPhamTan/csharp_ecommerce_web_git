const ConfirmPopup = ({ message, onConfirm, onCancel }) => {
  return (
    <div className="confirm-popup">
      <div className="confirm-popup-content">
        <p>{message}</p>
        <div>
          <button onClick={onConfirm}>Confirm</button>
          <button onClick={onCancel}>Cancel</button>
        </div>
      </div>
    </div>
  );
};

export default ConfirmPopup;
