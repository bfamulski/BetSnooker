import { CircularProgress } from '@mui/material';
import { display, justifyContent, twCls } from 'style';

export interface SpinnerProps {
  size?: number;
  color?: 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning' | undefined;
}

export const Spinner = ({ size, color }: SpinnerProps) => {
  return (
    <div className={twCls(display('flex'), justifyContent('justify-center'))}>
      <CircularProgress size={size ?? 40} color={color ?? 'inherit'} />
    </div>
  );
};
