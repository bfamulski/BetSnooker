import { useNavigate } from 'react-router';
import { useSetRecoilState } from 'recoil';
import { authUserState } from 'state';
import { Credentials } from 'models';
import { authenticationService, localStorageKeys, localStorageService } from 'services';

export const useAuth = () => {
  const setAuthUser = useSetRecoilState(authUserState);
  const navigate = useNavigate();

  const signIn = async (credentials: Credentials) => {
    const user = await authenticationService.login(credentials);
    if (user) {
      user.authData = window.btoa(`${credentials.username}:${credentials.password}`);
      setAuthUser(user);
      localStorageService.set(localStorageKeys.loggedInUserData, user);
      navigate('/');
    }
  };

  const signOut = () => {
    localStorageService.remove(localStorageKeys.loggedInUserData);
    setAuthUser(null);
    navigate('/login');
  };

  return { signIn, signOut };
};
