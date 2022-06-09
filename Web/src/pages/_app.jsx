import React from 'react';
import { SessionProvider } from 'next-auth/react';
import { ChakraProvider } from '@chakra-ui/react';
import Head from 'next/head';

export default function App({
  Component,
  pageProps: { session, ...pageProps },
}) {
  return (
    <SessionProvider
      session={session}
      refetchInterval={Number(process.env.NEXTAUTH_REFRESH_INTERVAL)}
    >
      <ChakraProvider>
        <Head>
          <title>ARMazing</title>
        </Head>
        <Component {...pageProps} />
      </ChakraProvider>
    </SessionProvider>
  );
}