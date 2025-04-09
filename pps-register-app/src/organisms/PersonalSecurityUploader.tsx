import personalPropertySecuritiesApi from '../api/personalPropertySecuritiesApi';
import FileUpload from '../atoms/FileUpload';

const PersonalSecurityUploader = () => {
  const { mutateAsync, isPending } = personalPropertySecuritiesApi.useUpload();

  const onFileSelected = async (file: File) => {
    await mutateAsync(file);
  };

  return <FileUpload onFileSelected={onFileSelected} />;
};

export default PersonalSecurityUploader;
