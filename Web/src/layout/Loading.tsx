import React from 'react';
import { Box, Flex, Heading, Stack, Spinner, Text } from '@chakra-ui/react';

/**
 * Default loading page when the content is loading.
 */
export default function Loading() {
  return (
    <Flex minH='100vh' align='center' justify='center' bg='gray.50'>
      <Stack spacing={8} mx='auto' maxW='lg' py={12} px={6}>
        <Box rounded='lg' bg='white' boxShadow='lg' p={8}>
          <Stack align='center'>
            <Heading fontSize='4xl'>ARMazing</Heading>
            <Text fontSize='sm' color='gray.600'>
              Loading webpage... Please give it a moment
            </Text>
            <Spinner />
          </Stack>
        </Box>
      </Stack>
    </Flex>
  );
}
