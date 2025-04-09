import ClientSelector from '../organisms/ClientSelector';

const Nav = () => {
  return (
    <nav className="bg-white shadow-lg p-4 flex items-center">
      <h1 className="text-2xl">Personal Property Securities Register</h1>
      <ClientSelector className="ml-auto" />
    </nav>
  );
};

export default Nav;
