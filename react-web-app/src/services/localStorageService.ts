export const localStorageKeys = {
  loggedInUserData: 'loggedInUserData',
} as const;

type Nullable<T> = T | null;

type LocalStorageKey = keyof typeof localStorageKeys;

export const localStorageService = {
  has(key: LocalStorageKey): boolean {
    const value = window.localStorage.getItem(key);
    if (value) {
      return true;
    }
    return false;
  },
  get<T>(key: LocalStorageKey): Nullable<T> {
    const value = window.localStorage.getItem(key);
    if (value) {
      const json = JSON.parse(value);
      return json as T;
    }
    return null;
  },
  set<T>(key: LocalStorageKey, value: T) {
    if (value !== null) {
      window.localStorage.setItem(key, JSON.stringify(value));
    }
  },
  remove(key: LocalStorageKey) {
    window.localStorage.removeItem(key);
  },
  clear() {
    window.localStorage.clear();
  },
};
