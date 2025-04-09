import { Fragment } from 'react/jsx-runtime';
import personalPropertySecuritiesApi from '../api/personalPropertySecuritiesApi';
import LoaderWrapper from '../molecules/LoaderWrapper';
import { ArrowPathIcon } from '@heroicons/react/24/outline';

const UploadsList = ({ className }: { className?: string }) => {
  const { data: uploads, isFetching } =
    personalPropertySecuritiesApi.useGetUploads();

  const invalidateUploads =
    personalPropertySecuritiesApi.useInvalidateUploads();

  return (
    <article className={className}>
      <h2 className="text-2xl font-semibold mb-4">
        Uploads{' '}
        <ArrowPathIcon
          className="inline-block cursor-pointer size-6"
          onClick={invalidateUploads}
        />
      </h2>
      <LoaderWrapper loading={isFetching}>
        <section className="grid grid-cols-5 min-h-10 border border-gray-200 rounded-2xl divide-x divide-y divide-gray-200 overflow-hidden">
          {uploads &&
            uploads?.map((upload) => (
              <Fragment key={upload.id}>
                <h3 className="col-span-5 p-2 text-primary-500 font-semibold">
                  {upload.fileName}
                </h3>
                {upload.submitted ? (
                  <>
                    <RegistrationItem
                      title="Submitted"
                      value={upload.submitted}
                    />
                    <RegistrationItem title="Invalid" value={upload.invalid} />
                    <RegistrationItem
                      title="Processed"
                      value={upload.processed}
                    />
                    <RegistrationItem title="Updated" value={upload.updated} />
                    <RegistrationItem title="Added" value={upload.added} />
                  </>
                ) : (
                  <section className="col-span-5 p-2 text-gray-500">
                    <p>File is being processed. Please check back later.</p>
                  </section>
                )}
              </Fragment>
            ))}
          {uploads?.length === 0 && (
            <section className="col-span-5 p-2 text-gray-500">
              <p className="text-lg text-center">No uploads found</p>
            </section>
          )}
        </section>
      </LoaderWrapper>
    </article>
  );
};

export default UploadsList;

const RegistrationItem = <T extends number>({
  title,
  value,
}: {
  title: string;
  value: T | undefined;
}) => {
  return (
    <section className="flex flex-col items-center p-2">
      <h4>{title}</h4>
      <p className="text-sm text-gray-500">{value}</p>
    </section>
  );
};
