import { ChangeEvent, FormEvent, useEffect, useState } from 'react';
import { useNavigate } from 'react-router';
import { useRecoilValue } from 'recoil';
import { authUserState } from 'state';
import { useAuth } from 'hooks';
import { ErrorHelper } from 'helpers';
import { Credentials } from 'models';
import { Button } from '@mui/material';
import {
  alignItems,
  backgroundColor,
  borderColor,
  borderRadius,
  borderWidth,
  display,
  flexDirection,
  fontSize,
  fontWeight,
  gap,
  height,
  justifyContent,
  margin,
  maxWidth,
  padding,
  twCls,
  width,
} from 'style';
import { Notification, Spinner } from 'components';

export const LoginPage = () => {
  const authUser = useRecoilValue(authUserState);
  const { signIn } = useAuth();

  const navigate = useNavigate();

  const [credentials, setCredentials] = useState<Credentials>({} as Credentials);

  const [isInProgress, setIsInProgress] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleChange = (event: ChangeEvent<HTMLInputElement>) => {
    const name = event.target.name;
    const value = event.target.value;
    setCredentials({ ...credentials, [name]: value });
  };

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();

    setIsInProgress(true);
    try {
      await signIn(credentials);
    } catch (error) {
      console.error(error);
      const errorMessage = ErrorHelper.formatErrorMessage(error);
      setError(errorMessage);
    } finally {
      setIsInProgress(false);
    }
  };

  useEffect(() => {
    if (authUser) {
      navigate('/');
    }
  }, [authUser, navigate]);

  return (
    <>
      {!authUser && (
        <div className={twCls(display('flex'), justifyContent('justify-center'), width('w-screen'), padding('px-4'))}>
          <div
            className={twCls(
              width('w-full'),
              borderColor('border-gray-300'),
              borderWidth('border'),
              borderRadius('rounded'),
              maxWidth('max-w-md'),
              margin('mt-4', 'md:mt-8', 'lg:mt-12')
            )}
          >
            <h3
              className={twCls(
                display('flex'),
                justifyContent('justify-center'),
                alignItems('items-center'),
                height('h-12'),
                backgroundColor('bg-gray-100'),
                borderWidth('border-b'),
                borderColor('border-gray-300'),
                fontSize('text-2xl'),
                fontWeight('font-medium')
              )}
            >
              BetSnooker
            </h3>

            <form onSubmit={handleSubmit}>
              <div
                className={twCls(
                  display('flex'),
                  flexDirection('flex-col'),
                  gap('gap-y-4'),
                  padding('px-4'),
                  margin('mt-4')
                )}
              >
                <div className={twCls(display('flex'), flexDirection('flex-col'))}>
                  <label htmlFor="username">User</label>
                  <input
                    id="username"
                    type="text"
                    name="username"
                    value={credentials.username || ''}
                    onChange={handleChange}
                    className={twCls(
                      borderWidth('border'),
                      borderRadius('rounded'),
                      borderColor('border-gray-300'),
                      height('h-10'),
                      padding('px-2')
                    )}
                  />
                </div>

                <div className={twCls(display('flex'), flexDirection('flex-col'))}>
                  <label htmlFor="password">Password</label>
                  <input
                    id="password"
                    type="password"
                    name="password"
                    value={credentials.password || ''}
                    onChange={handleChange}
                    className={twCls(
                      borderWidth('border'),
                      borderRadius('rounded'),
                      borderColor('border-gray-300'),
                      height('h-10'),
                      padding('px-2')
                    )}
                  />
                </div>
                <Notification message={error!} severity="error" onClose={() => setError(null)} />
                <Button
                  variant="contained"
                  type="submit"
                  color="success"
                  className={twCls(backgroundColor('bg-green-700'))}
                  disabled={!credentials.username || !credentials.password || isInProgress}
                >
                  <div className={twCls(display('flex'), alignItems('items-center'), gap('gap-x-2'))}>
                    {isInProgress && <Spinner size={20} />}
                    Login
                  </div>
                </Button>
              </div>
            </form>

            <div
              className={twCls(
                display('flex'),
                flexDirection('flex-col'),
                justifyContent('justify-center'),
                padding('px-4', 'pt-8', 'pb-4')
              )}
            >
              Do not have an account?
              <Button variant="outlined" color="warning" disabled>
                Register
              </Button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};
