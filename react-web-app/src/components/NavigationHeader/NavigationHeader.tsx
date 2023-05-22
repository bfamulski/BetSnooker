import { MouseEvent, useState } from 'react';
import { NavLink } from 'react-router-dom';
import { useRecoilValue } from 'recoil';
import { authUserState } from 'state';
import { useAuth } from 'hooks';
import { Avatar, Button, ListItemIcon, Menu, MenuItem } from '@mui/material';
import { Logout } from '@mui/icons-material';
import {
  alignItems,
  backgroundColor,
  combineCls,
  display,
  fontSize,
  fontWeight,
  gap,
  height,
  justifyContent,
  padding,
  textColor,
  textTransform,
  twCls,
  width,
} from 'style';

const navPages = [
  { key: 'home', path: '/', pageDisplayName: 'Home' },
  { key: 'bets', path: '/bets', pageDisplayName: 'Bets' },
];

const navLinkClassName = twCls(padding('px-2'));
const activeLinkClassName = combineCls(navLinkClassName, twCls(fontWeight('font-bold')));
const setActiveLinkClassName = ({ isActive }: { isActive: boolean }) =>
  isActive ? activeLinkClassName : navLinkClassName;

export const NavigationHeader = () => {
  const authUser = useRecoilValue(authUserState);
  const { signOut } = useAuth();

  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
  const open = Boolean(anchorEl);
  const handleClick = (event: MouseEvent<HTMLElement>) => setAnchorEl(event.currentTarget);
  const handleClose = () => setAnchorEl(null);

  return (
    <header
      className={twCls(
        width('w-full'),
        display('flex'),
        justifyContent('justify-between'),
        backgroundColor('bg-green-700'),
        textColor('text-white'),
        alignItems('items-center')
      )}
    >
      <nav className={twCls(display('flex'))}>
        {navPages.map(({ key, path, pageDisplayName }) => (
          <NavLink key={key} to={path} className={setActiveLinkClassName}>
            {pageDisplayName}
          </NavLink>
        ))}
      </nav>
      <Button className={twCls(display('flex'), gap('gap-x-2'))} onClick={handleClick}>
        <span className={twCls(textColor('text-white'), textTransform('normal-case'), fontSize('text-base'))}>
          {authUser?.username}
        </span>
        <Avatar className={twCls(height('h-7'), width('w-7'), backgroundColor('bg-red-600'))}>
          {authUser?.username.slice(0, 1)}
        </Avatar>
      </Button>
      <Menu anchorEl={anchorEl} open={open} onClose={handleClose} onClick={handleClose}>
        <MenuItem onClick={signOut} className={twCls(height('h-8'))}>
          <ListItemIcon>
            <Logout fontSize="small" />
          </ListItemIcon>
          Logout
        </MenuItem>
      </Menu>
    </header>
  );
};
