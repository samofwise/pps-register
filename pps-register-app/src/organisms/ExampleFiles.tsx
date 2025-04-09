import CsvIcon from '../assets/csv.svg?react';
import clsx from 'clsx';
import { getFileName } from '../utils/fileUtils';

import sample1 from '../assets/sampleData/1_Sample data.csv';
import sample2 from '../assets/sampleData/motor mystery records.csv';
import sample3 from '../assets/sampleData/mmr UPDATED.csv';
import sample4 from '../assets/sampleData/spg_squad_rollcall.csv';
import sample5 from '../assets/sampleData/the_final_vin.csv';
import sample6 from '../assets/sampleData/vinny_the_vehicle_batch.csv';

import invalid1 from '../assets/sampleData/Invalid Sample 1.csv';
import invalid2 from '../assets/sampleData/invalid_sample_2.csv';
import invalid3 from '../assets/sampleData/invalid_file.csv';
import invalid4 from '../assets/sampleData/invalid_file2.csv';

const validUrls = [sample1, sample2, sample3, sample4, sample5, sample6];

const invalidUrls = [invalid1, invalid2, invalid3, invalid4];

const SampleDataFiles = ({ className }: { className: string }) => {
  return (
    <>
      <section className={clsx('flex gap-2 overflow-x-auto', className)}>
        {validUrls.map((url, i) => (
          <SampleDataFile key={i} url={url} />
        ))}
      </section>
      <section className={clsx('flex gap-2 overflow-x-auto', className)}>
        {invalidUrls.map((url, i) => (
          <SampleDataFile key={i} url={url} isInvalid />
        ))}
      </section>
    </>
  );
};

const SampleDataFile = ({
  url,
  isInvalid = false,
}: {
  url: string;
  isInvalid?: boolean;
}) => {
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
        ''
      )}
    >
      <CsvIcon className={clsx('w-15', isInvalid && 'text-red-500')} />
      <p className="text-xs text-gray-500 flex">
        <span className="truncate">{name}</span>
        <span className="">{extension}</span>
      </p>
    </article>
  );
};

const getFileNameParts = (fileName: string) => {
  const [name, extension] = fileName.split(/[.?]/);
  return { name, extension: `.${extension}` };
};

export default SampleDataFiles;
