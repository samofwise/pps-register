import { DragEvent } from 'react';
import personalPropertySecuritiesApi from '../api/personalPropertySecuritiesApi';
import FileUpload from '../atoms/FileUpload';
import { getFile } from '../utils/fileUtils';

const PersonalPropertySecurityUploader = () => {
  const { mutateAsync, isPending } = personalPropertySecuritiesApi.useUpload();

  const invalidateUploads =
    personalPropertySecuritiesApi.useInvalidateUploads();

  const uploadFile = async (file: File) => {
    await mutateAsync(file);
    invalidateUploads();
  };

  const handleDrop = async (e: DragEvent<HTMLElement>) => {
    e.preventDefault();
    const url = e.dataTransfer.getData('text/plain');
    if (url) {
      const file = await getFile(url);
      uploadFile(file);
    }
  };

  return (
    <FileUpload
      onFileSelected={uploadFile}
      onDrop={handleDrop}
      isUploading={isPending}
    />
  );
};

export default PersonalPropertySecurityUploader;
