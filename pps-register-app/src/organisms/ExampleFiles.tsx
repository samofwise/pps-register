import CsvIcon from '../assets/csv.svg?react';
import sampleData1 from '../assets/Sample data.csv';
import sampleData2 from '../assets/Sample data copy.csv';
import clsx from 'clsx';
import { getFileName } from '../utils/fileUtils';

const urls = [sampleData1, sampleData2];

const SampleDataFiles = ({ className }: { className: string }) => {
  return (
    <section className={`flex gap-2 ${className}`}>
      {urls.map((url, i) => (
        <SampleDataFile key={i} url={url} />
      ))}
    </section>
  );
};

const SampleDataFile = ({ url }: { url: string }) => {
  const handleDragStart = (e: React.DragEvent<HTMLDivElement>) => {
    e.dataTransfer.setData('text/plain', url);
  };
  const fileName = getFileName(url);
  const { name, extension } = getFileNameParts(fileName);

  return (
    <article
      draggable
      onDragStart={handleDragStart}
      className={clsx(
        'flex flex-col items-center gap-2 p-2 rounded-2xl text-center',
        'hover:bg-primary-50',
        'cursor-grab',
        'overflow-x-auto'
      )}
    >
      <CsvIcon className="w-15" />
      <p className="text-xs text-gray-500 max-w-30 flex">
        <span className="truncate">{name}</span>
        <span className="">{extension}</span>
      </p>
    </article>
  );
};

const getFileNameParts = (fileName: string) => {
  const [name, extension] = fileName.split('.');
  return { name, extension: `.${extension}` };
};

export default SampleDataFiles;
