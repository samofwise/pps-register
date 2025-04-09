import Nav from './Nav';

interface LayoutProps {
  children: React.ReactNode;
}

const Layout = ({ children }: LayoutProps) => {
  return (
    <>
      <Nav />
      <main className="flex-grow">{children}</main>
    </>
  );
};

export default Layout;
