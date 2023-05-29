/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./src/**/*.{js,jsx,ts,tsx}'],
  theme: {
    extend: {
      colors: {
        'primary-background': 'rgb(var(--color-primary-background))',
        'primary-background-from': 'rgb(var(--color-primary-background-from))',
        'primary-background-to': 'rgb(var(--color-primary-background-to))',
        'primary-text': 'rgb(var(--color-primary-text))',
        'secondary-background': 'rgb(var(--color-secondary-background))',
        'secondary-background-from': 'rgb(var(--color-secondary-background-from))',
        'secondary-background-to': 'rgb(var(--color-secondary-background-to))',
        'secondary-text': 'rgb(var(--color-secondary-text))',
        avatar: 'rgb(var(--color-avatar))',
      },
    },
  },
  plugins: [],
};
