import { Fragment } from 'react/jsx-runtime';
import personalPropertySecuritiesApi from '../api/personalPropertySecuritiesApi';
import LoaderWrapper from '../molecules/LoaderWrapper';
import { ArrowPathIcon } from '@heroicons/react/24/outline';

const PersonalPropertySecuritiesList = ({
  className,
}: {
  className?: string;
}) => {
  const { data: personalPropertySecurities, isFetching } =
    personalPropertySecuritiesApi.useGetPersonalPropertySecurities();

  const invalidatePersonalPropertySecurities =
    personalPropertySecuritiesApi.useInvalidatePersonalPropertySecurities();

  return (
    <article className={className}>
      <h2 className="text-2xl font-semibold mb-4">
        Personal Property Securities{' '}
        <ArrowPathIcon
          className="inline-block cursor-pointer size-6"
          onClick={invalidatePersonalPropertySecurities}
        />
      </h2>
      <LoaderWrapper loading={isFetching}>
        <div className="overflow-x-scroll w-full">
          <section className="grid grid-cols-[auto_auto_auto_auto_auto_auto_auto_auto] min-h-10 border border-gray-200 rounded-2xl divide-x divide-y divide-gray-200 overflow-x-auto whitespace-nowrap">
            <h3 className="p-2 font-semibold">Grantor First Name</h3>
            <h3 className="p-2 font-semibold">Grantor Middle Names</h3>
            <h3 className="p-2 font-semibold">Grantor Last Name</h3>
            <h3 className="p-2 font-semibold">VIN</h3>
            <h3 className="p-2 font-semibold">Registration Start Date</h3>
            <h3 className="p-2 font-semibold">Registration Duration</h3>
            <h3 className="p-2 font-semibold">SPG ACN</h3>
            <h3 className="p-2 font-semibold">SPG Organization Name</h3>
            {personalPropertySecurities &&
              personalPropertySecurities?.map((pps) => (
                <Fragment key={pps.id}>
                  <p className="p-2">{pps.grantorFirstName}</p>
                  <p className="p-2">{pps.grantorMiddleNames}</p>
                  <p className="p-2">{pps.grantorLastName}</p>
                  <p className="p-2">{pps.vin}</p>
                  <p className="p-2">
                    {new Date(pps.registrationStartDate).toLocaleDateString()}
                  </p>
                  <p className="p-2">{pps.registrationDuration}</p>
                  <p className="p-2">{pps.spgAcn}</p>
                  <p className="p-2">{pps.spgOrganizationName}</p>
                </Fragment>
              ))}
            {personalPropertySecurities?.length === 0 && (
              <section className="col-span-8 p-2 text-gray-500">
                <p className="text-lg text-center">No uploads found</p>
              </section>
            )}
          </section>
        </div>
      </LoaderWrapper>
    </article>
  );
};

export default PersonalPropertySecuritiesList;
