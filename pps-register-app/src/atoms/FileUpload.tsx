import { useCallback } from 'react';
import { useDropzone } from 'react-dropzone';
import { Button } from './Button';
import clsx from 'clsx';

const FileUpload = ({ onFileSelected: onFileSelect }: FileUploadProps) => {
  const onDrop = useCallback(
    (acceptedFiles: File[]) => {
      const file = acceptedFiles[0];
      if (file && file.type === 'text/csv') {
        onFileSelect(file);
      }
    },
    [onFileSelect]
  );

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: {
      'text/csv': ['.csv'],
    },
    maxFiles: 1,
    multiple: false,
  });

  return (
    <article
      {...getRootProps()}
      className={clsx(
        'border-2 border-dashed rounded-lg p-8 text-center transition-colors cursor-pointer text-primary-500',
        isDragActive
          ? 'border-current bg-[color-mix(in_srgb,currentColor_10%,white)]'
          : 'border-gray-300',
        'hover:bg-current-50'
      )}
    >
      <input {...getInputProps()} />
      <div className="flex flex-col gap-2 text-gray-600">
        <p className="text-lg font-medium">
          {isDragActive
            ? 'Drop the CSV file here'
            : 'Drag and drop your CSV file here'}
        </p>
        <p className="text-sm">or</p>
        <Button className="self-center" variant="outline">
          Select File
        </Button>
      </div>
    </article>
  );
};

export default FileUpload;

interface FileUploadProps {
  onFileSelected: (file: File) => void;
}
