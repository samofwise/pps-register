import Loader from '../atoms/Loader';

const LoaderWrapper = ({
  children,
  loading,
}: {
  children: React.ReactNode;
  loading: boolean;
}) => {
  if (!loading) return children;

  return (
    <div className="relative">
      <div className="absolute inset-0 bg-white opacity-50 flex justify-center items-center">
        <Loader />
      </div>
      {children}
    </div>
  );
};

export default LoaderWrapper;
