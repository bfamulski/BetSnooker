import { atom } from 'recoil';
import { User } from 'models';
import { localStorageKeys, localStorageService } from 'services';

export const authUserState = atom({
  key: 'authUser',
  default: localStorageService.get<User | null>(localStorageKeys.loggedInUserData),
});
