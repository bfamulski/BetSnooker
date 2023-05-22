import { Collapse, Alert, IconButton } from '@mui/material';
import { Close as CloseIcon } from '@mui/icons-material';

export interface NotificationProps {
  message: string;
  severity: 'error' | 'warning' | 'info' | 'success';
  onClose: () => void;
}

export const Notification = ({ message, severity, onClose }: NotificationProps) => {
  return (
    <>
      <Collapse in={message != null && message !== ''}>
        <Alert
          severity={severity}
          action={
            <IconButton aria-label="close" color="inherit" size="small" onClick={onClose}>
              <CloseIcon fontSize="inherit" />
            </IconButton>
          }
        >
          {message}
        </Alert>
      </Collapse>
    </>
  );
};
