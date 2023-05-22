import { ReactNode } from 'react';
import { Navigate } from 'react-router';
import { useRoutes } from 'react-router-dom';
import { useRecoilValue } from 'recoil';
import { authUserState } from 'state';
import { MainLayout } from 'layouts';
import { BetsPage, HomePage, LoginPage } from 'pages';

interface ProtectedRouteProps {
  children: ReactNode;
}

const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const authUser = useRecoilValue(authUserState);

  if (!authUser) {
    return <Navigate to="/login" />;
  }

  return <>{children}</>;
};

const App = () => {
  const routes = useRoutes([
    {
      path: '/login',
      element: <LoginPage />,
    },
    {
      element: (
        <ProtectedRoute>
          <MainLayout />
        </ProtectedRoute>
      ),
      children: [
        {
          path: '/',
          element: (
            <ProtectedRoute>
              <HomePage />
            </ProtectedRoute>
          ),
        },
        {
          path: '/bets',
          element: (
            <ProtectedRoute>
              <BetsPage />
            </ProtectedRoute>
          ),
        },
      ],
    },
    {
      path: '*',
      element: <Navigate to="/" />,
    },
  ]);

  return routes;
};

export default App;
