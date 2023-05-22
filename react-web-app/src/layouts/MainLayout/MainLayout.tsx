import { Outlet } from 'react-router-dom';
import { NavigationHeader } from 'components';

export const MainLayout = () => {
  return (
    <>
      <NavigationHeader />
      <Outlet />
    </>
  );
};
