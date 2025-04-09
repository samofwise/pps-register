import UploadsList from '../organisms/UploadsList';
import PersonalPropertySecurityUploader from '../organisms/PersonalPropertySecurityUploader';
import PersonalPropertySecuritiesList from '../organisms/PersonalPropertySecuritiesList';
import SampleDataFiles from '../organisms/ExampleFiles';

const Home = () => {
  return (
    <article className="h-full flex flex-col max-w-4xl mx-auto p-8">
      <div className="mb-6">
        <h2 className="text-4xl font-semibold mb-4">Upload to the Register</h2>
        <p className="text-gray-600">
          Upload your personal property security documentation to register it
          with the PPSR.
        </p>
      </div>

      <PersonalPropertySecurityUploader />

      <SampleDataFiles className="mt-10" />

      <UploadsList className="mt-10" />

      <PersonalPropertySecuritiesList className="mt-10" />
    </article>
  );
};

export default Home;
