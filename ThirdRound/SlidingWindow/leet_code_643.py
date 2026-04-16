from typing import List


def findMaxAvg(nums: List[int], k: int) -> float:
    max_avg = 0
    cur_sum = 0

    for i in range(k):
        cur_sum += nums[i]

    max_avg = cur_sum / k

    for i in range(k, len(nums)):
        cur_sum += nums[i]
        cur_sum -= nums[i - k]

        avg = cur_sum / k
        max_avg = max(max_avg, avg)

    return max_avg


print(findMaxAvg([7, 4, 5, 8, 8, 3, 9, 8, 7, 6], 7))